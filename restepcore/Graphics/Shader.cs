using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System;

namespace restep.Graphics
{
    internal class Shader : IDisposable
    {
        /// <summary>
        /// Public name of the shader for meshes to refer to them by
        /// Also used for exception messages
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Handles to each shader object
        /// </summary>
        private int VSHandle;
        private int FSHandle;
        private int progHandle;

        /// <summary>
        /// Dictionary mapping uniform names to their corresponding location in the shader
        /// </summary>
        private Dictionary<string, int> uniformLocations;
        
        /// <summary>
        /// Whether or not the shader has been compiled and is usable
        /// </summary>
        public bool Loaded { get; private set; }

        /// <summary>
        /// Whether or not the shader is enabled for a renderer's use
        /// </summary>
        public bool Enabled { get; set; }

        public Shader(string shaderName, string shaderPath = "")
        {
            Name = shaderName;
            VSHandle = 0;
            FSHandle = 0;
            Loaded = false;
            if (!string.IsNullOrWhiteSpace(shaderPath))
            {
                LoadShader(shaderPath);
            }
            uniformLocations = new Dictionary<string, int>();
        }

        #region ~shader program loading~

        /// <summary>
        /// attempts to load both vertex and fragment shaders in one go from the specified files
        /// </summary>
        /// <param name="path">the path to the shader excluding its extension (.vs/.fs)</param>
        public void LoadShader(string path)
        {
            try
            {
                createShaders();
                string vertexSource, fragmentSource;

                //read vertex and fragment shader code from their respective files (shader.vs and shader.fs)
                using (StreamReader vfr = new StreamReader(path + ".vs"))
                {
                    vertexSource = vfr.ReadToEnd();
                }
                using (StreamReader ffr = new StreamReader(path + ".fs"))
                {
                    fragmentSource = ffr.ReadToEnd();
                }

                //compile each
                compileShader(VSHandle, vertexSource);
                compileShader(FSHandle, fragmentSource);

                //link them
                linkShaders();
                Loaded = true;
            }
            catch (Exception e)
            {
                destroyExistingShaders();
                destroyShaderProgram();
                Loaded = false;
                throw e;
            }
        }

        /// <summary>
        /// attempts to load both vertex and fragment shaders in one go using the specified code provided
        /// </summary>
        /// <param name="vertexSource">source for which vertex shader will compile</param>
        /// <param name="fragmentSource">source for which fragment shader will compile</param>
        public void LoadShader(string vertexSource, string fragmentSource)
        {
            try
            {
                createShaders();

                compileShader(VSHandle, vertexSource);
                compileShader(FSHandle, fragmentSource);

                linkShaders();
                Loaded = true;
            }
            catch (Exception e)
            {
                destroyExistingShaders();
                destroyShaderProgram();
                Loaded = false;
                throw e;
            }
        }

        private void destroyExistingShaders()
        {
            if (VSHandle != 0)
            {
                GL.DeleteShader(VSHandle);
            }
            if (FSHandle != 0)
            {
                GL.DeleteShader(FSHandle);
            }

            VSHandle = FSHandle = 0;
        }

        private void destroyShaderProgram()
        {
            if(progHandle != 0)
            {
                GL.DeleteProgram(progHandle);
            }

            progHandle = 0;
        }

        private void createShaders()
        {
            //make sure shaders don't already exist, avoid memory leaks
            destroyExistingShaders();

            //create our shaders
            VSHandle = GL.CreateShader(ShaderType.VertexShader);
            FSHandle = GL.CreateShader(ShaderType.FragmentShader);
            
            if (VSHandle == 0)
            {
                throw new Exception("Failed to create vertex shader for shader named " + Name + "! GL Error code: " + GL.GetError());
            }
            if (FSHandle == 0)
            {
                throw new Exception("Failed to create fragment shader for shader named " + Name + "! GL Error code: " + GL.GetError());
            }
        }

        private void compileShader(int shader, string shaderCode)
        {
            //set the shader's source code to our code and compile it
            GL.ShaderSource(shader, shaderCode);
            GL.CompileShader(shader);

            int status;

            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);

            //when the shader doesn't compile correctly (if the code provided doesn't work)
            //GL_TRUE & GL_FALSE please opentk?
            if(status == 0)
            {
                string error;
                GL.GetShaderInfoLog(shader, out error);
                throw new Framework.Exceptions.ShaderCompileException("Shader named " + Name + " failed to compile. Error message: " + error);
            }
        }

        private void linkShaders()
        {
            //ensure a program doesn't already exist for some reason, avoiding memory leaks
            destroyShaderProgram();

            //create instance of our shader program
            progHandle = GL.CreateProgram();

            if(progHandle == 0)
            {
                throw new Exception("Failed to create shader program for shader named " + Name + "! GL Error code: " + GL.GetError());
            }
            
            //attach shaders to our program for linking
            GL.AttachShader(progHandle, VSHandle);
            GL.AttachShader(progHandle, FSHandle);

            GL.LinkProgram(progHandle);

            //once shader program is linked, we don't need the individual shaders anymore
            GL.DetachShader(progHandle, VSHandle);
            GL.DetachShader(progHandle, FSHandle);

            destroyExistingShaders();
        }

        public void UseShader()
        {
            if(Loaded && Enabled)
            {
                GL.UseProgram(progHandle);
            }
        }

        #endregion

        #region ~uniform mapping~
        
        public void AddUniform(string uniformName)
        {
            if(!Loaded)
            {
                throw new Framework.Exceptions.InvalidShaderException("Shader program has not been created or has failed to create for shader named " + Name + "!");
            }

            //find the uniform in our program
            int uniformLocation = GL.GetUniformLocation(progHandle, uniformName);

            if (uniformLocation == -1)
            {
                throw new Framework.Exceptions.UniformNotFoundException("Failed to find uniform named " + uniformName + " for shader named " + Name + "! GL Error code: " + GL.GetError());
            }

            //map the uniform's name to its location
            uniformLocations.Add(uniformName, uniformLocation);
        }

        public void SetUniformMat3(string uniformName, OpenTK.Matrix3 matRef)
        {
            int location;

            if(!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.UniformMatrix3(location, false, ref matRef);
        }

        public void SetUniformInt(string uniformName, int i)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform1(location, i);
        }

        public void SetUniformFloat(string uniformName, float fl)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform1(location, fl);
        }

        public void SetUniformVec2(string uniformName, float x, float y)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform2(location, x, y);
        }

        public void SetUniformVec3(string uniformName, float x, float y, float z)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform3(location, x, y, z);
        }

        public void SetUniformVec4(string uniformName, float x, float y, float z, float w)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform4(location, x, y, z, w);
        }

        #endregion

        public void Dispose()
        {
            destroyExistingShaders();
            destroyShaderProgram();
            Loaded = false;
        }

        ~Shader()
        {
            Dispose();
        }
    }
}