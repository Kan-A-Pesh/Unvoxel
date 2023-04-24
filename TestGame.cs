using System.Numerics;
using Unvoxel.GameLoop;
using Unvoxel.Rendering.Display;
using Unvoxel.Rendering.Shaders;
using GLFW;
using static OpenGL.GL;

namespace Unvoxel
{
    class TestGame : Unvoxel.GameLoop.Game
    {
        string title;

        uint vao;
        uint vbo;

        Shader shader;

#pragma warning disable CS8618
        public TestGame(int initialWindowWidth, int initialWindowHeight, string initialWindowTitle) :
            base(initialWindowWidth, initialWindowHeight, initialWindowTitle)
        {
            this.title = initialWindowTitle;
        }
#pragma warning restore CS8618

        protected override void Initialize()
        {

        }

        protected unsafe override void LoadContent()
        {
            // Load shaders
            shader = new Shader(
                "in vec3 pos;void main(){gl_Position = vec4(pos, 1.0);}",
                "#include \"main.hlsl\""
            );

            Console.WriteLine(shader.fragmentCode);

            shader.Load();

            // Create VAO and VBO
            vao = glGenVertexArray();
            vbo = glGenBuffer();

            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            float[] vertices = { 0 };

            // Get the address of the first vertices
            fixed (float* v = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
            }

            glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), (void*)0);
            glEnableVertexAttribArray(0);

            glBindBuffer(GL_ARRAY_BUFFER, 0);
            glBindVertexArray(0);

            shader.Use();
        }

        protected unsafe override void Render()
        {
            string FPS = MathF.Round(1 / GameTime.DeltaTime, 2) + " FPS";
            int width = this.InitialWindowWidth;
            int height = this.InitialWindowHeight;

            Glfw.SetWindowTitle(DisplayManager.Window, title + " - " + FPS + " - " + width + "x" + height);

            glClearColor(0, 0, 0, 0);
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            glEnable(GL_DEPTH_TEST);

            shader.SetVector2("screenRes", new Vector2(width, height));

            float[] uniqueTriangle = {
                -1, 3, 0,
                -1, -1, 0,
                3, -1, 0
            };

            fixed (float* v = &uniqueTriangle[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * uniqueTriangle.Length, v, GL_STATIC_DRAW);
            }

            glBindBuffer(GL_ARRAY_BUFFER, vbo);
            glBindVertexArray(vao);
            glDrawArrays(GL_TRIANGLES, 0, 3);

            Glfw.SwapBuffers(DisplayManager.Window);
        }
        protected override void Update()
        {
            if (Glfw.GetKey(DisplayManager.Window, Keys.Escape) == InputState.Press)
            {
                Glfw.SetWindowShouldClose(DisplayManager.Window, true);
            }
        }
    }
}