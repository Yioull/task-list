using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace Tasks
{
    [TestFixture]
    public sealed class ApplicationTest
    {
        public const string PROMPT = "> ";

        private FakeConsole console;
        private CancellationTokenSource _cancellationToken;
        private IDisposable _runTask;

        [SetUp]
        public void StartTheApplication()
        {
            this.console = new FakeConsole();
            var taskList = new TaskList(console);
            _cancellationToken = new CancellationTokenSource();
            _runTask = System.Threading.Tasks.Task.Run(() => taskList.Run(_cancellationToken.Token));
        }

        [TearDown]
        public void KillTheApplication()
        {
            _cancellationToken.Cancel();
            _runTask.Dispose();
        }

        [Test, Timeout(1000)]
        public void ItWorks()
        {
            Execute("show");

            Execute("add project secrets");
            Execute("add task secrets Eat more donuts.");
            Execute("add task secrets Destroy all humans.");

            Execute("show");
            ReadLines(
                "secrets",
                "    [ ] 1: Eat more donuts.",
                "    [ ] 2: Destroy all humans.",
                ""
            );

            Execute("add project training");
            Execute("add task training Four Elements of Simple Design");
            Execute("add task training SOLID");
            Execute("add task training Coupling and Cohesion");
            Execute("add task training Primitive Obsession");
            Execute("add task training Outside-In TDD");
            Execute("add task training Interaction-Driven Design");

            Execute("check 1");
            Execute("check 3");
            Execute("check 5");
            Execute("check 6");

            Execute("show");
            ReadLines(
                "secrets",
                "    [x] 1: Eat more donuts.",
                "    [ ] 2: Destroy all humans.",
                "",
                "training",
                "    [x] 3: Four Elements of Simple Design",
                "    [ ] 4: SOLID",
                "    [x] 5: Coupling and Cohesion",
                "    [x] 6: Primitive Obsession",
                "    [ ] 7: Outside-In TDD",
                "    [ ] 8: Interaction-Driven Design",
                ""
            );

            Execute("quit");
        }

        private void Execute(string command)
        {
            Read(PROMPT);
            Write(command);
        }

        private void Read(string expectedOutput)
        {
            var length = expectedOutput.Length;
            var actualOutput = console.RetrieveOutput(expectedOutput.Length);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        private void ReadLines(params string[] expectedOutput)
        {
            foreach (var line in expectedOutput)
            {
                Read(line + Environment.NewLine);
            }
        }

        private void Write(string input)
        {
            console.SendInput(input + Environment.NewLine);
        }
    }
}