using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using restep.Framework.Logging;
using restep.Framework.Exceptions;
using restep.Framework.ResourceManagement;

namespace restep.Graphics.Shaders
{
    /// <summary>
    /// Represents a compiled GLSL shader program
    /// </summary>
    internal class Shader : IDisposable
    {
        public bool Loaded { get; private set; }

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
        /// Attempts to load both vertex and fragment shaders in one go from the specified files
        /// </summary>
        /// <param name="path">The path to the shader excluding its extension (.vs/.fs)</param>
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
            catch (LoggedException e)
            {
                destroyExistingShaders();
                destroyShaderProgram();
                Loaded = false;
                throw e;
            }
            catch (Exception e)
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "Shader", MessageType.Error, e.Message, true);
                destroyExistingShaders();
                destroyShaderProgram();
                Loaded = false;
                throw e;
            }
        }

        /// <summary>
        /// Attempts to load both vertex and fragment shaders in one go using the specified code provided
        /// </summary>
        /// <param name="vertexSource">Source for which vertex shader will compile</param>
        /// <param name="fragmentSource">Source for which fragment shader will compile</param>
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
            catch (LoggedException e)
            {
                destroyExistingShaders();
                destroyShaderProgram();
                Loaded = false;
                throw e;
            }
            catch (Exception e)
            {
                MessageLogger.LogMessage(MessageLogger.RENDER_LOG, "Shader", MessageType.Error, e.Message, true);
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
                throw new LoggedException($"Failed to create vertex shader for shader named {Name}! GL Error code: {GL.GetError()}", MessageLogger.RENDER_LOG, "Shader");
            }
            if (FSHandle == 0)
            {
                throw new LoggedException($"Failed to create fragment shader for shader named {Name}! GL Error code: {GL.GetError()}", MessageLogger.RENDER_LOG, "Shader");
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
                throw new LoggedException($"Shader named {Name} failed to compile. Error message: {error}", MessageLogger.RENDER_LOG, "Shader");
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
                throw new LoggedException($"Failed to create shader program for shader named {Name}! GL Error code: {GL.GetError()}", MessageLogger.RENDER_LOG, "Shader");
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

        /// <summary>
        /// Tells GL to use the program if loaded and enabled
        /// </summary>
        public void UseShader()
        {
            if(Loaded && Enabled)
            {
                GL.UseProgram(progHandle);
            }
        }

        #endregion

        #region ~uniform mapping~

        /// <summary>
        /// Search compiled shader for a uniform 
        /// <para>Throws Exception on failure</para>
        /// </summary>
        /// <param name="uniformName">The name of the uniform to search</param>
        /// <param name="type">The type of the uniform being mapped</param>
        /// <param name="container">The type of class which contains the uniform to be mapped</param>
        public void AddUniform(string uniformName)
        {
            if(!Loaded)
            {
                throw new LoggedException($"Shader program has not been created or has failed to create for shader named {Name}!", MessageLogger.RENDER_LOG, "Shader");
            }

            //find the uniform in our program
            int uniformLocation = GL.GetUniformLocation(progHandle, uniformName);

            if (uniformLocation == -1)
            {
                throw new LoggedException($"Failed to find uniform named {uniformName} for shader named {Name}! GL Error code: {GL.GetError()}", MessageLogger.RENDER_LOG, "Shader");
            }

            //map the uniform's name to its location
            uniformLocations.Add(uniformName, uniformLocation);
        }

        /// <summary>
        /// Sets a mat3 uniform
        /// </summary>
        /// <param name="uniformName">Name of the mat3</param>
        /// <param name="matRef">Matrix3 to copy from</param>
        public void SetUniformMat3(string uniformName, Matrix3 matRef)
        {
            int location;

            if(!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.UniformMatrix3(location, false, ref matRef);
        }

        /// <summary>
        /// Sets an integer uniform
        /// </summary>
        /// <param name="uniformName">Name of the int</param>
        /// <param name="i">Int to copy from</param>
        public void SetUniformInt(string uniformName, int i)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform1(location, i);
        }

        /// <summary>
        /// Sets a float uniform
        /// </summary>
        /// <param name="uniformName">Name of the float</param>
        /// <param name="fl">Float to copy from</param>
        public void SetUniformFloat(string uniformName, float fl)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform1(location, fl);
        }

        /// <summary>
        /// Sets a vec2 uniform
        /// </summary>
        /// <param name="uniformName">Name of the vec2</param>
        /// <param name="x">X component of the vec2</param>
        /// <param name="y">Y component of the vec2</param>
        public void SetUniformVec2(string uniformName, float x, float y)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform2(location, x, y);
        }

        /// <summary>
        /// Sets a vec3 uniform
        /// </summary>
        /// <param name="uniformName">Name of the vec3</param>
        /// <param name="x">X component of the vec3</param>
        /// <param name="y">Y component of the vec3</param>
        /// <param name="z">Z component of the vec3</param>
        public void SetUniformVec3(string uniformName, float x, float y, float z)
        {
            int location;

            if (!uniformLocations.TryGetValue(uniformName, out location))
            {
                return;
            }

            GL.Uniform3(location, x, y, z);
        }


        /// <summary>
        /// Sets a vec4 uniform
        /// </summary>
        /// <param name="uniformName">Name of the vec4</param>
        /// <param name="x">X component of the vec4</param>
        /// <param name="y">Y component of the vec4</param>
        /// <param name="z">Z component of the vec4</param>
        /// <param name="w">W component of the vec4</param>
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
    }
}