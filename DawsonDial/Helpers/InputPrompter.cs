namespace DawsonDial.Helpers
{
    /// <summary>
    /// Provides static methods for prompting the user for input.
    /// </summary>
    public static class InputPrompter
    {
        /// <summary>
        /// Prompts the user for input and returns the entered text.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <returns>The user's input.</returns>
        public static string Prompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? string.Empty;
        }

        /// <summary>
        /// Prompts the user for a password and returns the entered text.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <returns>The user's input.</returns>
        public static string PromptPassword(string message)
        {
            Console.Write(message);
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        /// <summary>
        /// Prompts the user for an integer input within a specified range.
        /// </summary>
        /// <param name="prompt">The prompt message to display.</param>
        /// <param name="min">The minimum allowed value (inclusive).</param>
        /// <param name="max">The maximum allowed value (inclusive).</param>
        /// <param name="defaultValue">The default value to use if input is empty.</param>
        /// <returns>The user's input as an integer.</returns>
        public static int PromptInt(string prompt, int? min = null, int? max = null, int? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                // Use default if input is empty and default is provided
                if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                    return defaultValue.Value;

                if (int.TryParse(input, out int value))
                {
                    if (min.HasValue && value < min.Value)
                    {
                        Console.WriteLine($"Value must be at least {min.Value}.");
                        continue;
                    }

                    if (max.HasValue && value > max.Value)
                    {
                        Console.WriteLine($"Value must not exceed {max.Value}.");
                        continue;
                    }
                    return value;
                }

                Console.WriteLine("Invalid number. Please enter an integer.");
            }
        }

        /// <summary>
        /// Prompts the user for a date and returns the entered date.
        /// </summary>
        /// <param name="prompt">The message to display to the user.</param>
        /// <param name="min">The minimum date allowed.</param>
        /// <returns>The user's input.</returns>
        public static DateOnly PromptDate(string prompt, DateOnly? min = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (DateOnly.TryParse(input, out var result))
                {
                    if (min != null && result < min)
                    {
                        Console.WriteLine($"Date must be after {min.Value:MM/dd/yyyy}");
                        continue;
                    }
                    return result;
                }

                Console.WriteLine("Invalid date format.");
            }
        }

        /// <summary>
        /// Prompts the user for a time and returns the entered time.
        /// </summary>
        /// <param name="prompt">The message to display to the user.</param>
        /// <returns>The user's input.</returns>
        public static TimeOnly PromptTime(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (TimeOnly.TryParse(input, out var result))
                    return result;

                Console.WriteLine("Invalid time format.");
            }
        }

        /// <summary>
        /// Prompts the user for a confirmation and returns the entered confirmation.
        /// </summary>
        /// <param name="prompt">The message to display to the user.</param>
        /// <returns>True if the user confirms, false otherwise.</returns>
        public static bool Confirm(string prompt)
        {
            Console.Write($"{prompt} (y/n): ");
            string? input = Console.ReadLine()?.Trim().ToLower();

            return input == "y" || input == "yes";
        }
    }
}
