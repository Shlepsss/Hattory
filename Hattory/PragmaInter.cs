using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using sus = Cosmos.System;
using Cosmos.System.Graphics;
using System.Threading;

namespace Hattory
{
    public class Variable
    {
        public string Name = "";
        public string Value = "";
        public string Type = "string"; // "string", "number", "boolean"
    }

    public class PragmaInterpritator
    {
        public static List<Variable> variables = new List<Variable>();
        public static List<string> code = new List<string>();

        public static bool inIf = false;
        public static bool inElse = false;
        public static bool ifTrue = false;
        public static int currentLine = 0;

        public static void Execute()
        {
            try
            {
                //variables.Clear();
                inIf = false;
                inElse = false;
                ifTrue = false;
                currentLine = 0;

                while (currentLine < code.Count)
                {
                    string line = code[currentLine].Trim();
                    currentLine++;

                    if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                    {
                        continue;
                    }

                    ProcessLine(line);
                }
            }
            catch (Exception e)
            {
                FpsShower.Msg($"Error in line {currentLine}", e.Message, false);
                FpsShower.playSound = true;
            }
            finally
            {
                //Kernel.op = "";
            }
        }

        private static void ProcessLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;

            // Обработка условий
            if (inElse || inIf)
            {
                ProcessConditionalBlock(line);
                return;
            }

            // Основные команды
            ProcessCommand(line);
        }

        private static void ProcessConditionalBlock(string line)
        {
            if (line.Equals("endcond", StringComparison.OrdinalIgnoreCase))
            {
                inIf = false;
                inElse = false;
                ifTrue = false;
            }
            else if (line.Equals("else", StringComparison.OrdinalIgnoreCase))
            {
                if (inIf)
                {
                    inElse = !ifTrue;
                    inIf = false;
                }
            }
            else if ((inIf && ifTrue) || inElse)
            {
                ProcessCommand(line);
            }
        }

        private static void ProcessCommand(string line)
        {
            if (line.StartsWith("text ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessTextCommand(line);
            }
            else if (line.StartsWith("rect ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessRectCommand(line);
            }
            else if (line.StartsWith("circle ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessCircleCommand(line);
            }
            else if (line.StartsWith("pixel ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessPixelCommand(line);
            }
            else if (line.Equals("shutdown", StringComparison.OrdinalIgnoreCase))
            {
                sus.Power.Shutdown();
            }
            else if (line.Equals("reboot", StringComparison.OrdinalIgnoreCase))
            {
                sus.Power.Reboot();
            }
            else if (line.Equals("stop", StringComparison.OrdinalIgnoreCase))
            {
                Kernel.op = "";
            }
            else if (line.StartsWith("msg ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessMessageCommand(line);
            }
            else if (line.StartsWith("beep ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessBeepCommand(line);
            }
            else if (line.StartsWith("var ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessVariableDeclaration(line);
            }
            else if (line.StartsWith("set ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessVariableAssignment(line);
            }
            else if (line.StartsWith("if ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessIfStatement(line);
            }
            else if (line.StartsWith("plus ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessMathOperation(line, "plus");
            }
            else if (line.StartsWith("minus ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessMathOperation(line, "minus");
            }
            else if (line.StartsWith("multiply ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessMathOperation(line, "multiply");
            }
            else if (line.StartsWith("divide ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessMathOperation(line, "divide");
            }
            else if (line.StartsWith("power ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessMathOperation(line, "power");
            }
            else if (line.StartsWith("sqrt ", StringComparison.OrdinalIgnoreCase))
            {
                ProcessSqrtCommand(line);
            }
            else
            {
                throw new Exception($"Invalid command: {line}");
            }
        }

        private static void ProcessTextCommand(string line)
        {
            string[] parts = line.Substring(5).Split("||");
            if (parts.Length < 4) throw new Exception("Invalid text command");

            int x = GetNumberValue(parts[0]);
            int y = GetNumberValue(parts[1]);
            string text = GetStringValue(parts[2]);
            Color color = GetColorValue(parts[3]);

            Otrisovka.Write(text, x, y, color);
        }

        private static void ProcessRectCommand(string line)
        {
            string[] parts = line.Substring(5).Split("||");
            if (parts.Length < 5) throw new Exception("Invalid rect command");

            int x = GetNumberValue(parts[0]);
            int y = GetNumberValue(parts[1]);
            int w = GetNumberValue(parts[2]);
            int h = GetNumberValue(parts[3]);
            Color color = GetColorValue(parts[4]);

            Kernel.canvas.DrawFilledRectangle(color, x, y, w, h);
        }

        private static void ProcessCircleCommand(string line)
        {
            string[] parts = line.Substring(7).Split("||");
            if (parts.Length < 4) throw new Exception("Invalid circle command");

            int x = GetNumberValue(parts[0]);
            int y = GetNumberValue(parts[1]);
            int r = GetNumberValue(parts[2]);
            Color color = GetColorValue(parts[3]);

            Kernel.canvas.DrawFilledCircle(color, x, y, r);
        }

        private static void ProcessPixelCommand(string line)
        {
            string[] parts = line.Substring(6).Split("||");
            if (parts.Length < 3) throw new Exception("Invalid pixel command");

            int x = GetNumberValue(parts[0]);
            int y = GetNumberValue(parts[1]);
            Color color = GetColorValue(parts[2]);

            Kernel.canvas.DrawPoint(color, x, y);
        }

        private static void ProcessMessageCommand(string line)
        {
            string[] parts = line.Substring(4).Split("||");
            if (parts.Length < 3) throw new Exception("Invalid msg command");

            string message = GetStringValue(parts[0]);
            string title = GetStringValue(parts[1]);
            bool isInfo = parts[2].Trim().Equals("i", StringComparison.OrdinalIgnoreCase);

            FpsShower.Msg(message, title, isInfo);
            FpsShower.playSound = true;
        }

        private static void ProcessBeepCommand(string line)
        {
            string[] parts = line.Substring(5).Split("||");
            if (parts.Length < 2) throw new Exception("Invalid beep command");

            int freq = GetNumberValue(parts[0]);
            int dur = GetNumberValue(parts[1]);

            Cosmos.System.PCSpeaker.Beep((uint)freq, (uint)dur);
        }

        private static void ProcessVariableDeclaration(string line)
        {
            string[] parts = line.Substring(4).Split("||");
            string varName = parts[0];
            string value = parts[1];
            string type = "";
            if (FindVariable(varName) == null)
            {
                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Substring(1, value.Length - 2);
                    type = "string";
                }
                else if (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                         value.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    value = value.ToLower();
                    type = "boolean";
                }
                else if (IsNumber(value))
                {
                    type = "number";
                }
                else if (value.StartsWith("&&"))
                {
                    // Копируем значение из другой переменной
                    Variable sourceVar = FindVariable(value.Substring(2));
                    if (sourceVar != null)
                    {
                        value = sourceVar.Value;
                        type = sourceVar.Type;
                    }
                }
                else
                {
                    throw new Exception("Invalid var type!");
                }
                variables.Add(new Variable { Name = varName, Value = value, Type = type });
            }
        }

        private static void ProcessVariableAssignment(string line)
        {
            string[] parts = line.Substring(4).Split("||");
            if (parts.Length < 2) throw new Exception("Invalid set command");

            string varName = parts[0].Trim();
            string value = parts[1].Trim();

            Variable var = FindVariable(varName);
            if (var == null) throw new Exception($"Variable {varName} not found");

            // Определяем тип значения
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                var.Value = value.Substring(1, value.Length - 2);
                var.Type = "string";
            }
            else if (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                     value.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                var.Value = value.ToLower();
                var.Type = "boolean";
            }
            else if (IsNumber(value))
            {
                var.Value = value;
                var.Type = "number";
            }
            else if (value.StartsWith("&&"))
            {
                // Копируем значение из другой переменной
                Variable sourceVar = FindVariable(value.Substring(2));
                if (sourceVar != null)
                {
                    var.Value = sourceVar.Value;
                    var.Type = sourceVar.Type;
                }
            }
            else
            {
                throw new Exception("Invalid var type!");
            }
        }

        private static void ProcessIfStatement(string line)
        {
            string condition = line.Substring(3).Trim();
            inIf = true;

            if (condition.StartsWith("mouse cia", StringComparison.OrdinalIgnoreCase))
            {
                string[] coords = condition.Substring(10).Split("||");
                if (coords.Length < 4) throw new Exception("Invalid mouse condition");

                int x = GetNumberValue(coords[0]);
                int y = GetNumberValue(coords[1]);
                int w = GetNumberValue(coords[2]);
                int h = GetNumberValue(coords[3]);

                ifTrue = Kernel.Click(x, y, w, h);
            }
            else
            {
                string[] parts = condition.Split(' ');
                if (parts.Length < 3) throw new Exception("Invalid if condition");

                string varName = parts[0];
                string op = parts[1];
                string compareValue = parts[2];

                Variable var = FindVariable(varName);
                if (var == null) throw new Exception($"Variable {varName} not found");

                ifTrue = EvaluateCondition(var, op, compareValue);
            }
        }

        private static bool EvaluateCondition(Variable variable, string op, string compareValue)
        {
            if (variable.Type == "number")
            {
                int varValue = int.Parse(variable.Value);
                int compareVal = GetNumberValue(compareValue);

                return op switch
                {
                    ">" => varValue > compareVal,
                    "<" => varValue < compareVal,
                    ">=" => varValue >= compareVal,
                    "<=" => varValue <= compareVal,
                    "==" => varValue == compareVal,
                    "!=" => varValue != compareVal,
                    _ => throw new Exception($"Unknown operator: {op}")
                };
            }
            else if (variable.Type == "string")
            {
                string compareStr = GetStringValue(compareValue);
                return op switch
                {
                    "==" => variable.Value == compareStr,
                    "!=" => variable.Value != compareStr,
                    _ => throw new Exception($"Unknown operator for string: {op}")
                };
            }
            else if (variable.Type == "boolean")
            {
                bool compareBool = compareValue.Equals("true", StringComparison.OrdinalIgnoreCase);
                bool varBool = variable.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
                return op switch
                {
                    "==" => varBool == compareBool,
                    "!=" => varBool != compareBool,
                    _ => throw new Exception($"Unknown operator for boolean: {op}")
                };
            }

            throw new Exception("Unsupported variable type");
        }

        private static void ProcessMathOperation(string line, string operation)
        {
            string[] parts = line.Substring(operation.Length + 1).Split("||");
            if (parts.Length < 2) throw new Exception("Invalid math operation");

            string varName = parts[0].Trim();
            string valueStr = parts[1].Trim();

            Variable var = FindVariable(varName);
            if (var == null || var.Type != "number")
                throw new Exception($"Number variable {varName} not found");

            int currentValue = int.Parse(var.Value);
            int value = GetNumberValue(valueStr);

            int result = operation switch
            {
                "plus" => currentValue + value,
                "minus" => currentValue - value,
                "multiply" => currentValue * value,
                "divide" => value == 0 ? throw new Exception("Division by zero") : currentValue / value,
                "power" => (int)Math.Pow(currentValue, value),
                _ => throw new Exception($"Unknown operation: {operation}")
            };

            var.Value = result.ToString();
        }

        private static void ProcessSqrtCommand(string line)
        {
            string varName = line.Substring(5).Trim();
            Variable var = FindVariable(varName);
            if (var == null || var.Type != "number")
                throw new Exception($"Number variable {varName} not found");

            int value = int.Parse(var.Value);
            var.Value = ((int)Math.Sqrt(value)).ToString();
        }

        private static Variable FindVariable(string name)
        {
            foreach (Variable var in variables)
            {
                if (var.Name == name) return var;
            }
            return null;
        }

        private static int GetNumberValue(string input)
        {
            input = input.Trim();

            if (input.StartsWith("&&"))
            {
                Variable var = FindVariable(input.Substring(2));
                if (var == null || var.Type != "number")
                    throw new Exception($"Number variable {input.Substring(2)} not found");
                return int.Parse(var.Value);
            }

            if (int.TryParse(input, out int result))
                return result;

            throw new Exception($"Invalid number: {input}");
        }

        private static string GetStringValue(string input)
        {
            input = input.Trim();

            if (input.StartsWith("&&"))
            {
                Variable var = FindVariable(input.Substring(2));
                if (var == null) throw new Exception($"Variable {input.Substring(2)} not found");
                return var.Value;
            }

            if (input.StartsWith("\"") && input.EndsWith("\""))
                return input.Substring(1, input.Length - 2);

            return input;
        }

        private static Color GetColorValue(string input)
        {
            string colorName = GetStringValue(input);
            try
            {
                return Color.FromName(colorName);
            }
            catch
            {
                throw new Exception($"Invalid color: {colorName}");
            }
        }

        private static bool IsNumber(string value)
        {
            return int.TryParse(value, out _);
        }
    }
}