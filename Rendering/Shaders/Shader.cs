using System.Diagnostics;
using System.Numerics;
using static OpenGL.GL;

namespace Unvoxel.Rendering.Shaders
{
    public class Shader
    {
        string vertexCode;
        public string fragmentCode;

        public uint ProgramID { get; set; }

        public Shader(string vertexCode, string fragmentCode)
        {
            this.vertexCode = CompileIncludes(vertexCode);
            this.fragmentCode = CompileIncludes(fragmentCode);
        }

        private string CompileIncludes(string inputCode)
        {
            List<string> includes = new List<string>();

            while (inputCode.Contains("#include"))
            {
                int start = inputCode.IndexOf("#include");
                int end = inputCode.IndexOf("\"", start + 10) + 1;

                string includePath = inputCode.Substring(start + 10, end - start - 11);
                includePath = "assets/Shaders/" + includePath;

                // Prevent infinite loop & duplicate includes
                if (includes.Contains(includePath))
                {
                    inputCode = inputCode.Replace(inputCode.Substring(start, end - start), "");
                    continue;
                }

                string includeCode = File.ReadAllText(includePath);
                inputCode = inputCode.Replace(inputCode.Substring(start, end - start), includeCode);
            }

            return inputCode;
        }

        public void Load()
        {
            uint vs, fs;
            vs = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vs, vertexCode);
            glCompileShader(vs);

            // Check status of shader compilation.
            int[] status = glGetShaderiv(vs, GL_COMPILE_STATUS, 1);

            if (status[0] == 0)
            {
                // Failed to compile
                string error = glGetShaderInfoLog(vs);
                Debug.WriteLine("Error compiling VERTEX shader: " + error);
            }

            fs = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fs, fragmentCode);
            glCompileShader(fs);

            // Check status of shader compilation.
            status = glGetShaderiv(fs, GL_COMPILE_STATUS, 1);

            if (status[0] == 0)
            {
                // Failed to compile
                string error = glGetShaderInfoLog(fs);
                Debug.WriteLine("Error compiling FRAGMENT shader: " + error);
            }

            ProgramID = glCreateProgram();
            glAttachShader(ProgramID, vs);
            glAttachShader(ProgramID, fs);

            glLinkProgram(ProgramID);

            // Delete shaders to free memory
            glDetachShader(ProgramID, vs);
            glDetachShader(ProgramID, fs);
            glDeleteShader(vs);
            glDeleteShader(fs);
        }

        public void Use()
        {
            glUseProgram(ProgramID);
        }

        public void SetMatrix4x4(string uniformName, Matrix4x4 mat4)
        {
            int location = glGetUniformLocation(ProgramID, uniformName);
            glUniformMatrix4fv(location, 1, false, GetMatrix4x4Values(mat4));
        }

        public void SetVector2(string uniformName, Vector2 vec2)
        {
            int location = glGetUniformLocation(ProgramID, uniformName);
            glUniform2fv(location, 1, new float[] { vec2.X, vec2.Y });
        }

        public void SetVector3(string uniformName, Vector3 vec3)
        {
            int location = glGetUniformLocation(ProgramID, uniformName);
            glUniform3fv(location, 1, new float[] { vec3.X, vec3.Y, vec3.Z });
        }


        private float[] GetMatrix4x4Values(Matrix4x4 m)
        {
            return new float[]
            {
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            };
        }

    }
}