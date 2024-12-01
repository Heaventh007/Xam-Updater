using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamUpdater.Properties;

namespace XamUpdater
{
    class Program
    {
        // Initialize our current directory variable.
        static readonly string CurrentDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";

        // Define our xextool image name.
        const string XexToolFile = "xextool.exe";

        static bool UpdateXamFile(string XamPath, string OutputDirectory) {
            Console.WriteLine("Update started.");

            File.WriteAllBytes($"{CurrentDirectory}xam.xex", Resources.xam);

            // Allocate a new update process.
            Process updateProcess = new Process() {
                // Allocate a new start info instance.
                StartInfo = new ProcessStartInfo() {
                    // Combine our current directory with the xextool file.
                    FileName = $"{CurrentDirectory}{XexToolFile}",

                    // Specify our arguments in xextool. -d Updates into a directory.
                    Arguments = $"-o \"{OutputDirectory}\" -p \"{XamPath}\" \"{CurrentDirectory}xam.xex\"",

                    UseShellExecute = false, // This doesn't work with no window processes.
                    CreateNoWindow = false, // Don't create a window, we'll do it in the background.

                    // Set our working directory to our current directory.
                    WorkingDirectory = CurrentDirectory
                }
            };

            // Attempt to start our update process.
            if (updateProcess.Start()) {
                // Wait for our update process to complete.
                updateProcess.WaitForExit();

                // Delete our temp original xam.
                File.Delete($"{CurrentDirectory}xam.xex");

                return true; // Return true, hopefully it succeded.
            }

            // The update process failed to start.
            return false;
        }

        static void PrintFinished(string Error) {
            Console.WriteLine(Error); // Write our error message to the console window.
            Console.WriteLine("Press any key to exit."); // Tell the user we're waiting for an input.

            Console.ReadKey(); // Pause exiting unti a key is pressed.
        }

        static bool InitializeDirectories(string OutputDirectory) {
            // Check if our output directory exists.
            if (!Directory.Exists(OutputDirectory))
                // Directory does not exist, attempt to create our output directory.
                Directory.CreateDirectory(OutputDirectory);

            // Check if both, our temporary directory, and output directory exists.
            return Directory.Exists(OutputDirectory);
        }

        static void Main(string[] args) {

            // Check if proper arguments have been passed in.
            if (args.Length < 1) {
                // Print out the proper usage.
                PrintFinished($"Usage:\n XamUpdater.exe (XexpPath)\n XamUpdater.exe (XexpPath) (OutDirectory)\n\nExample:\n XamUpdater.exe {CurrentDirectory}xam.xexp {CurrentDirectory}UpdateDir");
                return;
            }
            
            // Check if the xam file exists.
            if (!File.Exists(args[0])) {
                // Tell the user that the input xam file does not exist.
                PrintFinished($"The input file '{args[0]}' does not exist.");
                return;
            }

            // Check if xextool exists in the tool directory.
            if (!File.Exists($"{CurrentDirectory}{XexToolFile}")) {
                // Tell the user we've failed to find the xex tool file.
                PrintFinished($"xextool.exe does not exist in the tool's directory.");
                return;
            }

            // Define our output directory.
            string OutputDirectory = ((args.Length > 1) ? args[1] : $"{CurrentDirectory}UpdateDirectory");

            // Attempt to initialize our directories.
            if (!InitializeDirectories(OutputDirectory)) {
                // Tell the user one of the directories was not able to be created.
                PrintFinished($"Failed to initialize directories.");
                return;
            }

            // Attempt to Update the input xam file.
            if (UpdateXamFile(args[0], $"{OutputDirectory}\\updxam.xex")) {
                // Tell the user we've successfully updated, and moved the file to the output directory.
                PrintFinished("Updated successfully.");
                return;
            }

            // Tell the user we've failed somewhere.
            PrintFinished("Failed to update xam file.");
        }
    }
}
