using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Hjson;

namespace Obganism.Tests
{
	static class Tests
	{
		[Test]
		[TestCaseSource(nameof(TheTestCasesFromFileSystem))]
		public static void CasesFromFiles(string obo, JsonArray expected)
		{
			if (expected.Count == 1 && expected[0].Qo().ContainsKey("position"))
			{
				var actual = Assert.Throws<ObganismException>(() => ObganismDocument.Parse(obo));

				AssertObo(expected[0].Qo(), actual);
			}

			else
			{
				var actual = ObganismDocument.Parse(obo);

				AssertObo(expected, actual);
			}
		}

		static void AssertObo(JsonObject expected, ObganismException actual)
		{
			Assert.AreEqual(expected.Qi("position"), actual.Position, "Exception positions are not the same.");
			Assert.AreEqual(expected.Qs("message"), actual.Message, "Exception messages are not the same.");
		}

		static void AssertObo(JsonArray expected, IReadOnlyList<ObganismObject> actual)
		{
			Assert.AreEqual(expected?.Count ?? 0, actual.Count, "Objects are not the same count.");

			for (int i = 0; i < actual.Count; ++i)
				AssertObo(expected[i].Qo(), actual[i]);
		}

		static void AssertObo(JsonObject expected, ObganismObject actual)
		{
			AssertObo(expected.Qo("type"), actual.Type);
			AssertObo(expected.Qa("properties"), actual.Properties);
			AssertObo(expected.Qa("modifiers"), actual.Modifiers);
		}

		static void AssertObo(JsonObject expected, ObganismType actual)
		{
			Assert.AreEqual(expected.Qs("name"), actual.Name, "Type names are not the same.");
			AssertObo(expected.Qa("generics"), actual.Generics);
		}

		static void AssertObo(JsonArray expected, IReadOnlyList<ObganismType> actual)
		{
			Assert.AreEqual(expected?.Count ?? 0, actual.Count, "Generics are not the same count.");

			for (int i = 0; i < actual.Count; ++i)
				AssertObo(expected[i].Qo(), actual[i]);
		}
		
		static void AssertObo(JsonArray expected, IReadOnlyList<ObganismProperty> actual)
		{
			Assert.AreEqual(expected?.Count ?? 0, actual.Count, "Properties are not the same count.");

			for (int i = 0; i < actual.Count; ++i)
				AssertObo(expected[i].Qo(), actual[i]);
		}

		static void AssertObo(JsonObject expected, ObganismProperty actual)
		{
			Assert.AreEqual(expected.Qs("name"), actual.Name, "Property names are not the same.");
			AssertObo(expected.Qo("type"), actual.Type);
			AssertObo(expected.Qa("modifiers"), actual.Modifiers);
		}

		static void AssertObo(JsonArray expected, IReadOnlyList<ObganismModifier> actual)
		{
			Assert.AreEqual(expected?.Count ?? 0, actual.Count, "Modifiers are not the same count.");

			for (int i = 0; i < actual.Count; ++i)
				AssertObo(expected[i].Qo(), actual[i]);
		}

		static void AssertObo(JsonObject expected, ObganismModifier actual)
		{
			Assert.AreEqual(expected.Qs("name"), actual.Name, "Modifiers names are not the same.");
			AssertObo(expected.Qa("parameters"), actual.Parameters);
		}

		static void AssertObo(JsonArray expected, IReadOnlyList<ObganismModifierParameter> actual)
		{
			Assert.AreEqual(expected?.Count ?? 0, actual.Count, "Modifier parameters are not the same count.");

			for (int i = 0; i < actual.Count; ++i)
				AssertObo(expected[i].Qo(), actual[i]);
		}

		static void AssertObo(JsonObject expected, ObganismModifierParameter actual)
		{
			switch (expected.Qs("type"))
			{
				case "int": Assert.AreEqual(expected.Qi("value"), (actual as ObganismModifierParameter.Integer).Value, "Modifier parameters are not the same."); break;
				case "float": Assert.AreEqual((float)expected.Qd("value"), (actual as ObganismModifierParameter.Real).Value, "Modifier parameters are not the same."); break;
				case "string": Assert.AreEqual(expected.Qs("value"), (actual as ObganismModifierParameter.String).Value, "Modifier parameters are not the same."); break;
				case "name": Assert.AreEqual(expected.Qs("value"), (actual as ObganismModifierParameter.Name).Value, "Modifier parameters are not the same."); break;
				case "type": AssertObo(expected.Qo("value"), (actual as ObganismModifierParameter.Type).Value); break;
			}
		}

		static IEnumerable<object> TheTestCasesFromFileSystem()
		{
			foreach (var file in Directory.GetFiles("Cases", "*.txt", SearchOption.AllDirectories))
			{
				var fileContent = File.ReadAllText(file).Replace("\r\n", "\n").Replace('\r', '\n');
				var fileContentParts = fileContent.Split("\n\n////////////////\n\n");
				var obo = fileContentParts[0];
				var hjson = fileContentParts[1].TrimStart();

				if (char.IsLetter(hjson[0]))
					hjson = $"[{{\n { hjson } \n}}]";

				else if (hjson[0] == '{')
					hjson = $"[\n { hjson } \n]";

				var parsedHjson = HjsonValue.Parse(hjson);

				yield return new TestCaseData(obo, parsedHjson).SetName(Path.GetFileNameWithoutExtension(file));
			}
		}
	}
}
