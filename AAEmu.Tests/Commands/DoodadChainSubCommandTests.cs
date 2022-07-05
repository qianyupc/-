﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Models.Game.Units;
using AAEmu.Game.Scripts.Commands;
using AAEmu.Game.Utils.Scripts;
using Moq;
using Xunit;

namespace AAEmu.Tests.Commands
{
    public class DoodadChainSubCommandTests
    {
        [Fact]
        public void PreExecute_WhenChain_ShouldCallChainSubCommand()
        {
            var mockSubCommand = new Mock<ISubCommand>();
            var mockUnitCustomModelParams = new Mock<UnitCustomModelParams>(UnitCustomModelType.None);
            var mockCharacter = new Mock<Character>(mockUnitCustomModelParams.Object);

            var command = new TestCommand(new Dictionary<ISubCommand, string[]> 
            { 
                { 
                    mockSubCommand.Object, new string[]{ "sdf"} 
                } 
            });

            command.PreExecute(mockCharacter.Object, "test", new[] { "sdf","123" });

            mockSubCommand.Verify(s => s.PreExecute(It.IsIn(mockCharacter.Object), It.IsIn("sdf"), It.Is<string[]>(a => a.Length == 1 && a[0] == "123")));
        }

        [Fact]
        public void PreExecute_WhenChain_ShouldCallChainSubSubCommand()
        {
            var mockSubSubCommand = new Mock<ISubCommand>();
            var mockUnitCustomModelParams = new Mock<UnitCustomModelParams>(UnitCustomModelType.None);
            var mockCharacter = new Mock<Character>(mockUnitCustomModelParams.Object);

            var subCommand = new SubTestCommand(new Dictionary<ISubCommand, string[]>
            {
                {
                    mockSubSubCommand.Object, new string[]{ "second"}
                }
            });

            var command = new TestCommand(new Dictionary<ISubCommand, string[]>
            {
                {
                    subCommand, new string[]{ "first"}
                }
            });

            command.PreExecute(mockCharacter.Object, "test", new[] { "first", "second", "parameter1second", "parameter2second" });

            mockSubSubCommand.Verify(s => s.PreExecute(It.IsIn(mockCharacter.Object), It.IsIn("second"), It.Is<string[]>(a => a.Length == 2 && a[0] == "parameter1second" && a[1] == "parameter2second")));
        }

        [Fact]
        public void Execute_WhenOnlyCommand_ShouldNotThrowException()
        {
            var mockUnitCustomModelParams = new Mock<UnitCustomModelParams>(UnitCustomModelType.None);
            var mockCharacter = new Mock<Character>(mockUnitCustomModelParams.Object);

            var testCommand = new TestCommand(new Dictionary<ISubCommand, string[]>());
            testCommand.PreExecute(mockCharacter.Object, "doodad", new string[] { });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Execute_WhenSendingHelp_ShouldReturnHelpText(int numberOfSupportedCommands)
        {
            var mockCharacter = new Mock<ICharacter>();
            var supportedCommands = new Dictionary<ISubCommand, string[]>();
            var mockSubCommands = new List<Mock<ISubCommand>>();
            var expectedCommands = new List<string>();
            for (int i = 0; i < numberOfSupportedCommands; i++)
            {
                var mockSubCommand = new Mock<ISubCommand>();
                mockSubCommands.Add(mockSubCommand);

                supportedCommands.Add(mockSubCommand.Object, new string[] { $"command{i}" });
                expectedCommands.Add($"command{i}");
            }

            var testCommand = new TestCommand(supportedCommands);
            var testCommandPrefix = "Prefix";
            testCommand.PreExecute(mockCharacter.Object, "test", new string[] { "help" });

            mockCharacter.Verify(c => c.SendMessage(It.IsIn($"{testCommandPrefix} {testCommand.Description}")), Times.Once);
            mockCharacter.Verify(c => c.SendMessage(It.IsIn($"{testCommandPrefix} {testCommand.CallExample}")), Times.Once);
            mockCharacter.Verify(c => c.SendMessage(It.Is<string>(s => s.Contains($"{string.Join("||", expectedCommands)}"))), Times.Once);
            mockCharacter.Verify(c => c.SendMessage(It.Is<string>(s => s.Contains("For more details use"))), Times.Once);
        }


        public class TestCommand : SubCommandBase
        {
            public TestCommand(Dictionary<ISubCommand, string[]> register) : base(register)
            {
                Prefix = "Prefix";
                Description = "Mock Command";
                CallExample = "Help Message";
            }
        }

        public class SubTestCommand : SubCommandBase
        {
            public SubTestCommand(Dictionary<ISubCommand, string[]> register) : base(register) { }
        }
    }
}