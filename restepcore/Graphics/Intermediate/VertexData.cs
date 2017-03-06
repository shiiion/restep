using System;
using System.Collections;
using System.Collections.Generic;
using restep.Framework.Exceptions;
using restep.Framework.Logging;
using restep.Graphics.Renderables;
using restep.Framework.ResourceManagement;


namespace restep.Graphics.Intermediate
{
    /// <summary>
    /// Standard types used for variables in GLSL
    /// </summary>
    public enum LayoutQualifierType
    {
        Float = 1, Vec2 = 2, Vec3 = 3, Vec4 = 4
    }

    public abstract class DataFormat : IEnumerable<LayoutQualifierType>
    {
        protected List<LayoutQualifierType> bufferOrder = new List<LayoutQualifierType>();

        public DataFormat()
        {
            InitFormat();
        }

        public int[] GetSizes()
        {
            int[] attributes = new int[bufferOrder.Count];

            for (int a=0;a<attributes.Length;a++)
            {
                attributes[a] = (int)bufferOrder[a];
            }

            return attributes;
        }

        public int GetNumAttributes()
        {
            return bufferOrder.Count;
        }

        public int GetAttributeSizeAt(int index)
        {
            return (int)bufferOrder[index];
        }

        public int GetAttributeIndex(int arrayIndex)
        {
            int attributeIndex = 0;
            foreach(LayoutQualifierType lqt in bufferOrder)
            {
                if(arrayIndex - (int)lqt < 0)
                {
                    break;
                }
                arrayIndex -= (int)lqt;
                attributeIndex++;
            }
            return attributeIndex;
        }

        public int GetFormatSize()
        {
            int size = 0;
            foreach(LayoutQualifierType lqt in bufferOrder)
            {
                size += (int)lqt;
            }
            return size;
        }

        protected abstract void InitFormat();

        public void AppendType(LayoutQualifierType type)
        {
            bufferOrder.Add(type);
        }

        public LayoutQualifierType this[int index]
        {
            get
            {
                return bufferOrder[index];
            }

            set
            {
                bufferOrder[index] = value;
            }
        }

        public IEnumerator<LayoutQualifierType> GetEnumerator()
        {
            return bufferOrder.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return bufferOrder.GetEnumerator();
        }
    }

    public class VertexData : CountableResource
    {
        private class CustomBufferData : FlatMesh.BufferData
        {
            public DataFormat BufferOrder { get; set; }

            public CustomBufferData(DataFormat format)
            {
                BufferOrder = format;
            }

            public int OrderSize
            {
                get
                {
                    int sum = 0;
                    foreach (LayoutQualifierType t in BufferOrder)
                    {
                        sum += (int)t;
                    }
                    return sum;
                }
            }

            /// <summary>
            /// Decodes a string using BufferOrder as a key
            /// <para>vertex format: "v_data1_data2_data3..."</para>
            /// <para>index format: "i_index1_index2_index3"</para>
            /// </summary>
            /// <param name="bufferString"></param>
            public CustomBufferData(string[] dataPoints, DataFormat format)
            {
                BufferOrder = format;
                if (dataPoints.Length - 1 != OrderSize)
                {
                    throw new Exception("Invalid buffer data string!");
                }
                Data = new float[OrderSize];

                int orderIndex = 0;

                for (int a = 1; a < dataPoints.Length;)
                {
                    switch (BufferOrder[orderIndex])
                    {
                        case LayoutQualifierType.Float:
                            Data[a - 1] = float.Parse(dataPoints[a]);
                            break;
                        case LayoutQualifierType.Vec2:
                            Data[a - 1] = float.Parse(dataPoints[a]);
                            Data[a] = float.Parse(dataPoints[a + 1]);
                            break;
                        case LayoutQualifierType.Vec3:
                            Data[a - 1] = float.Parse(dataPoints[a]);
                            Data[a] = float.Parse(dataPoints[a + 1]);
                            Data[a + 1] = float.Parse(dataPoints[a + 2]);
                            break;
                        case LayoutQualifierType.Vec4:
                            Data[a - 1] = float.Parse(dataPoints[a]);
                            Data[a] = float.Parse(dataPoints[a + 1]);
                            Data[a + 1] = float.Parse(dataPoints[a + 2]);
                            Data[a + 2] = float.Parse(dataPoints[a + 3]);
                            break;
                    }
                    a += (int)BufferOrder[orderIndex];
                    orderIndex++;
                }
            }
        }

        public DataFormat BufferFormat { get; private set; }

        private CustomBufferData[] bufferArray;

        public float[][] BuffersOut { get; private set; }

        public int VertexCount { get; private set; }

        public VertexData(DataFormat format, string dataString = "")
        {
            BufferFormat = format;
            string[] lines = dataString.Replace("\r", "").Split('\n');
            try
            {
                parseBufferDataLines(lines);
            }
            catch (Exception e)
            {
                throw new LoggedException("Failed to intitialize vertex data: " + e.Message, MessageLogger.RENDER_LOG, "VertexData");
            }
        }

        private void parseBufferDataLines(string[] lines)
        {
            List<CustomBufferData> vertices = new List<CustomBufferData>();
            List<ushort> indices = new List<ushort>();

            VertexCount = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                string lineLower = line.ToLower();
                string[] dataPoints = lineLower.Split('_');
                switch (dataPoints[0])
                {
                    case "v":
                        vertices.Add(new CustomBufferData(dataPoints, BufferFormat));
                        VertexCount++;
                        break;
                }
            }

            bufferArray = vertices.ToArray();
            BuffersOut = FlatMesh.BufferData.SeparateArrays(bufferArray, BufferFormat);
        }

        public override void OnDestroy() { }

        /// <summary>
        /// buffer format string (must be first line of data string)
        /// type, type, type....
        /// </summary>
        /// <param name="formatString"></param>
        //private void parseBufferFormat(string formatString)
        //{
        //    string[] formats = formatString.Replace(" ", "").Split(',');
        //    foreach (string type in formats)
        //    {
        //        switch (type)
        //        {
        //            case "float":
        //                bufferFormat.Add(LayoutQualifierType.Float);
        //                break;
        //            case "vec2":
        //                CustomBufferData.BufferOrder.Add(LayoutQualifierType.Vec2);
        //                break;
        //            case "vec3":
        //                CustomBufferData.BufferOrder.Add(LayoutQualifierType.Vec3);
        //                break;
        //            case "vec4":
        //                CustomBufferData.BufferOrder.Add(LayoutQualifierType.Vec4);
        //                break;
        //        }
        //    }
        //}
    }
}
