using Infrastructure.Models;
using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ConsoleProgram
{
    public class App
    {
        private readonly IConfiguration _configuration;

        private List<User> users = InMemoryDatabase.Users;

        public App(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Run()
        {
            users = CredentialService.GetUsersFromConfigFile(_configuration.GetValue<string>("CredentialsFilePath"));
            Console.WriteLine("AMMC");

            var role = Authorize();

            switch (role)
            {
                case "user":
                    Console.WriteLine("User Menu");
                    UserMainMenu();
                    break;

                case "admin":
                    Console.WriteLine("Admin Menu");
                    AdminMainMenu();
                    break;

                case "security":
                    Console.WriteLine("Security Menu");
                    SecurityMainMenu();
                    break;
            }
        }

        private string Authorize()
        {
            while (true)
            {
                Console.WriteLine("Please Enter your Login");
                var login = Console.ReadLine();

                Console.WriteLine("Please Enter your Password:");
                var password = string.Empty;
                ConsoleKey key;
                do
                {
                    var keyInfo = Console.ReadKey(intercept: true);
                    key = keyInfo.Key;

                    if (key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        Console.Write("\b \b");
                        password = password[0..^1];
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        Console.Write("*");
                        password += keyInfo.KeyChar;
                    }
                } while (key != ConsoleKey.Enter);

                var user = users.Where(x => x.Login == login && x.Password == password).FirstOrDefault();
                if (user != null)
                {
                    Console.WriteLine("\nAccess granted");
                    return user.Role;
                }
                else
                {
                    Console.WriteLine("\nYour credentials are incorrect. Please try again alter");
                }
            }
        }

        private void UserMainMenu()
        {
            var mainMenu = true;
            while (mainMenu)
            {
                Console.WriteLine("Please enter 1 for encrypt file, 2 for decrypt file, 3 for monitoring, 4 for data transfer, 0 for end application");
                var userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        Console.WriteLine("File encrypting started");
                        mainMenu = true;
                        break;
                    case "2":
                        Console.WriteLine("File decrypting started");
                        mainMenu = true;
                        break;
                    case "3":
                        Console.WriteLine("Monitoring process started");
                        Monitoring();
                        break;
                    case "4":
                        Console.WriteLine("Data tranfer started");
                        DataTransfer();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        continue;
                }
            }
        }

        private void AdminMainMenu()
        {
            var mainMenu = true;
            while (mainMenu)
            {
                Console.WriteLine("Please enter 1 for encrypt file, 2 for decrypt file, 3 for monitoring, 4 for data transfer, 5 for configuration, 6 for change credentials, 0 for end application");
                var userInput = Console.ReadLine();


                switch (userInput)
                {
                    case "1":
                        Console.WriteLine("File encrypting started");
                        mainMenu = true;
                        break;
                    case "2":
                        Console.WriteLine("File decrypting started");
                        mainMenu = true;
                        break;
                    case "3":
                        Monitoring();
                        break;
                    case "4":
                        DataTransfer();
                        break;
                    case "5":
                        Configuration();
                        break;
                    case "6":
                        ChangeCred();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        continue;
                }
            }
        }

        private void SecurityMainMenu()
        {
            var mainMenu = true;
            while (mainMenu)
            {
                Console.WriteLine("Please enter 1 for encrypt file, 2 for decrypt file, 3 for monitoring, 4 for data transfer, 5 for configuration, 0 for end application");
                var userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        Console.WriteLine("File encrypting started");
                        mainMenu = true;
                        break;
                    case "2":
                        Console.WriteLine("File decrypting started");
                        mainMenu = true;
                        break;
                    case "3":
                        Monitoring();
                        break;
                    case "4":
                        DataTransfer();
                        break;
                    case "5":
                        Configuration();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        continue;
                }
            }
        }
                
        private void Monitoring()
        {
            Console.WriteLine("Please enter 1 for data transfer monitoring, 2 for monitoring the performance of the transmission device, 3 for connection monitoring, 0 to retrun to previous menu");

            var userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("data transfer monitoring");
                    return;
                case "2":
                    Console.WriteLine("monitoring the performance of the transmission device");
                    return;
                case "3":
                    Console.WriteLine("connection monitoring");
                    return;
                case "0":
                    return;
            }
        }

        private void DataTransfer()
        {
            Console.WriteLine("Please enter 1 for send data, 2 for receive data, 0 to retrun to previous menu");

            var userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    TelnetServer server = new TelnetServer(_configuration.GetValue<string>("EncryptedFilePath"));
                    server.SendPdfFile();
                    break;
                case "2":
                    TelnetClient client = new TelnetClient();
                    client.SavePdfFile(_configuration.GetValue<string>("RecievedFilePath"));
                    break;
                case "0":
                    return;
            }

        }

        private void Configuration()
        {
            Console.WriteLine("Please enter 1 for change default config, 2 for reset default config, 3 for create new config, 4 for load config, 0 to retrun to previous menu");

            var userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    ChangeConfig();
                    break;
                case "2":
                    ResetConfig();
                    break;
                case "3":
                    CreateConfig();
                    break;
                case "4":
                    LoadConfig();
                    break;
                case "0":
                    return;
            }
        }

        private void ChangeCred()
        {
            Console.WriteLine("Please enter 1 for change user credentials, 2 for change security credentials, 3 for change admin credentials, 4 for reset credentials, 0 to retrun to previous menu");

            var userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Please enter new login");
                    string login;
                    while (true)
                    {
                        login = Console.ReadLine();
                        if (login.Length < 3)
                        {
                            Console.WriteLine("Login should have lenght 3 or more");
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    Console.WriteLine("Please enter new password");
                    string password;
                    while (true)
                    {
                        password = Console.ReadLine();
                        if (password.Length < 3)
                        {
                            Console.WriteLine("Password should have lenght 3 or more");
                        }
                        else
                        {
                            break;
                        }
                    }

                    foreach (var user in users)
                    {
                        if (user.Role == "user")
                        {
                            user.Login = login;
                            user.Password = password;
                        }
                    }

                    CredentialService.UpdateCredentials(_configuration.GetValue<string>("CredentialsFilePath"), users);
                    Console.WriteLine("Credentials updated!!!");

                    return;
                case "2":
                    Console.WriteLine("Please enter new login");
                    string secLogin;
                    while (true)
                    {
                        secLogin = Console.ReadLine();
                        if (secLogin.Length < 3)
                        {
                            Console.WriteLine("Login should have lenght 3 or more");
                        }
                        else
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Please enter new password");
                    string secPassword;
                    while (true)
                    {
                        secPassword = Console.ReadLine();
                        if (secPassword.Length < 3)
                        {
                            Console.WriteLine("Password should have lenght 3 or more");
                        }
                        else
                        {
                            break;
                        }
                    }

                    foreach (var user in users)
                    {
                        if (user.Role == "security")
                        {
                            user.Login = secLogin;
                            user.Password = secPassword;
                        }
                    }

                    CredentialService.UpdateCredentials(_configuration.GetValue<string>("CredentialsFilePath"), users);
                    Console.WriteLine("Credentials updated!!!");

                    return;
                case "3":
                    Console.WriteLine("Please enter new login");
                    string admLogin;
                    while (true)
                    {
                        admLogin = Console.ReadLine();
                        if (admLogin.Length < 3)
                        {
                            Console.WriteLine("Login should have lenght 3 or more");
                        }
                        else
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Please enter new password");
                    string admPassword;
                    while (true)
                    {
                        admPassword = Console.ReadLine();
                        if (admPassword.Length < 3)
                        {
                            Console.WriteLine("Password should have lenght 3 or more");
                        }
                        else
                        {
                            break;
                        }
                    }

                    foreach (var user in users)
                    {
                        if (user.Role == "admin")
                        {
                            user.Login = admLogin;
                            user.Password = admPassword;
                        }
                    }

                    CredentialService.UpdateCredentials(_configuration.GetValue<string>("CredentialsFilePath"), users);
                    Console.WriteLine("Credentials updated!!!");

                    return;
                case "4":
                    while (true)
                    {
                        Console.WriteLine("This operation could not be  undone. Please press 1 to Continue. Press 2 to Cancel");
                        var desicion = Console.ReadLine();
                        if (desicion == "1")
                        {
                            users = InMemoryDatabase.Users;
                            CredentialService.UpdateCredentials(_configuration.GetValue<string>("CredentialsFilePath"), users);
                            Console.WriteLine("Credentials reseted!!!");
                            break;
                        }
                        else if (desicion == "2")
                        {
                            break;
                        }
                    }
                    return;
                case "0":
                    return;
            }
        }

        private void ChangeConfig()
        {
            string filePath = _configuration.GetValue<string>("SettingsFileFilePath"); // змінна для збереження шляху до файлу конфігурації
            string newTR = "transmitter";
            int newORF = 1;
            string newOFB = "operating_frequency_range_1";
            string newUC = "converter_is_not_using";
            int newLOFC = 1;
            string newSU = "up_sideband";
            int newIRF = 1;
            int newIRFA = 2;
            int newNIRFA = 2;
            int newХIRFAi = 0;
            int newYIRFAi = 2;
            int newHPFIRF = 1;
            int newNHPFIRF = 2;
            int newXHPFIRFi = 0;
            int newYHPFIRFi = 1;
            int newLPFIRF = 1;
            int newNLPFIRF = 2;
            int newXLPFIRFi = 0;
            int newYLPFIRFi = 1;
            int newBPFIRF = 1;
            int newNBPFIRF = 2;
            int newXBPFIRFi = 0;
            int newYBPFIRFi = 1;
            string newFS = "hardware_synthesizer";
            int newOFFS1 = 1;
            int newNOFFS1 = 2;
            int newXOFFS1i = 0;
            int newYOFFS1i = 1;
            string newPLL = "hardware_PLL";
            int newPLLLPF = 1;
            int newNPLLLPF = 2;
            int newXPLLLPFi = 0;
            int newYPLLLPFi = 1;
            int newSDTR = 1;
            string newIFB = "calculated";
            int newSF1 = 1;
            double newIFBLPF = 1;
            string newDE = "data_encryption_is_using";
            string newTE = "Type_1";
            string newKE = "key_exchange_using";
            string newEDK = "1";
            string newCC = "corrective_coding_is_using";
            string newTCC = "Type_1";
            string newSM = "ASK";
            string newWC = "channel_1";
            int newENPS = 1;
            int newNSC = 1;
            int newXSCPi = 1;
            int newYSCPi = 1;
            int newHPS = 1;
            int newHCS = 1;
            int newHFS = 1;
            int newDBL = 1;
            int newFL = 0;

            // Перевірка, чи файл існує перед спробою зчитування
            if (File.Exists(filePath))
            {
                try
                {
                    // Зчитування поточних значень параметрів
                    string[] lines = File.ReadAllLines(filePath);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("TR"))
                        {
                            string[] parts = line.Split('=');
                            newTR = parts[1];

                            // Показ варіантів та вибір нового значення TR
                            Console.WriteLine("Choose Transmitter or Receiver:");
                            Console.WriteLine("1 - transmitter");
                            Console.WriteLine("2 - receiver");
                            
                            Console.Write($"Enter new value Transmitter or Receiver ({newTR}): ");
                            int TR = int.Parse(Console.ReadLine());

                            switch (TR)
                            {
                                case 1:
                                    newTR = "transmitter";
                                    break;
                                case 2:
                                    newTR = "receiver";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default transmitter.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("ORF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int ORFValue);
                            Console.Write($"Enter new value Operating radio frequency (1...300000000000 Hz) ({ORFValue}): ");
                            int.TryParse(Console.ReadLine(), out newORF);
                        }
                        else if (line.StartsWith("OFB"))
                        {
                            string[] parts = line.Split('=');
                            newOFB = parts[1];

                            // Показ варіантів та вибір нового значення OFB
                            Console.WriteLine("Choose OFB:");
                            Console.WriteLine("1 - operating_frequency_range_1");
                            Console.WriteLine("2 - operating_frequency_range_2");
                            Console.WriteLine("3 - operating_frequency_range_3");
                            Console.WriteLine("4 - operating_frequency_range_4");
                            Console.WriteLine("5 - operating_frequency_range_5");

                            Console.Write($"Enter new OFB ({newOFB}): ");
                            string[] OFBs = Console.ReadLine().Split(',');
                            newOFB = "";

                            foreach (var OFB in OFBs)
                            {
                                switch (OFB.Trim())
                                {
                                    case "1":
                                        newOFB += "operating_frequency_range_1";
                                        break;
                                    case "2":
                                        newOFB += "operating_frequency_range_2";
                                        break;
                                    case "3":
                                        newOFB += "operating_frequency_range_3";
                                        break;
                                    case "4":
                                        newOFB += "operating_frequency_range_4";
                                        break;
                                    case "5":
                                        newOFB += "operating_frequency_range_5";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default operating_frequency_range_1.");
                                        break;
                                }
                            }
                        }
                        else if (line.StartsWith("UC"))
                        {
                            string[] parts = line.Split('=');
                            newUC = parts[1];

                            // Показ варіантів та вибір нового значення UC
                            Console.WriteLine("Choose value Using converter:");
                            Console.WriteLine("1 - converter_is_not_using");
                            Console.WriteLine("2 - up-converter_is_using");
                            Console.WriteLine("3 - down-converter_is_using");
                            
                            Console.Write($"Enter new value Using converter ({newUC}): ");
                            int UC = int.Parse(Console.ReadLine());

                            switch (UC)
                            {
                                case 1:
                                    newUC = "converter_is_not_using";
                                    break;
                                case 2:
                                    newUC = "up-converter_is_using";
                                    break;
                                case 3:
                                    newUC = "down-converter_is_using";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default value converter_is_not_using.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("LOFC"))
                        {
                            if (newUC == "up-converter_is_using" || newUC == "down-converter_is_using")
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int LOFCValue);
                                Console.Write($"Enter new value local oscillator frequency of the converter (1...300000000000 Hz) ({LOFCValue}): ");
                                int.TryParse(Console.ReadLine(), out newLOFC);
                            }
                        }
                        else if (line.StartsWith("SU"))
                        {
                            if (newUC == "up-converter_is_using")
                            {
                                string[] parts = line.Split('=');
                                newSU = parts[1];

                                // Показ варіантів та вибір нового значення SU
                                Console.WriteLine("Choose value Sideband usage:");
                                Console.WriteLine("1 - up_sideband");
                                Console.WriteLine("2 - down_sideband");

                                Console.Write($"Enter new value Sideband usage ({newSU}): ");
                                int SU = int.Parse(Console.ReadLine());

                                switch (SU)
                                {
                                    case 1:
                                        newSU = "up_sideband";
                                        break;
                                    case 2:
                                        newSU = "down_sideband";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value up_sideband.");
                                        break;
                                }
                            }
                        }
                        else if (line.StartsWith("IRF"))
                        {
                            if (newUC == "converter_is_not_using")
                            {
                                newIRF = newORF;
                            }
                            else if (newUC == "up-converter_is_using" && newSU == "up_sideband")
                            {
                                newIRF = newORF - newLOFC;
                            }
                            else if (newUC == "up-converter_is_using" && newSU == "down_sideband")
                            {
                                newIRF = newLOFC - newORF;
                            }
                            else if (newUC == "down-converter_is_using")
                            {
                                newIRF = newORF - newLOFC;
                            }
                        }
                        else if (line.StartsWith("IRFA"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int IRFAValue);
                            Console.Write($"Enter new value Intermediate radio frequency amplification (-100...100 dB) ({IRFAValue}): ");
                            int.TryParse(Console.ReadLine(), out newIRFA);
                        }
                        else if (line.StartsWith("NIRFA"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int NIRFAValue);
                            Console.Write($"Enter new value Number of calibration points for intermediate radio frequency amplification (2...100) ({NIRFAValue}): ");
                            int.TryParse(Console.ReadLine(), out newNIRFA);
                        }
                        else if (line.StartsWith("ХIRFAi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int ХIRFAiValue);
                            Console.Write($"Enter new value Coordinates of calibration points X (0...255) ({ХIRFAiValue}): ");
                            int.TryParse(Console.ReadLine(), out newХIRFAi);
                        }
                        else if (line.StartsWith("YIRFAi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int YIRFAiValue);
                            Console.Write($"Enter new value Coordinates of calibration points Y (-100...100 dB) ({YIRFAiValue}): ");
                            int.TryParse(Console.ReadLine(), out newYIRFAi);
                        }
                        else if (line.StartsWith("HPFIRF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int HPFIRFValue);
                            Console.Write($"Enter new value Cutoff frequency of the high-pass filter (1...100000000000 Hz) ({HPFIRFValue}): ");
                            int.TryParse(Console.ReadLine(), out newHPFIRF);
                        }
                        else if (line.StartsWith("NHPFIRF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int NHPFIRFValue);
                            Console.Write($"Enter new value Number of calibration points for cutoff frequency of the high-pass filter (2...100) ({NHPFIRFValue}): ");
                            int.TryParse(Console.ReadLine(), out newNHPFIRF);
                        }
                        else if (line.StartsWith("XHPFIRFi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int XHPFIRFiValue);
                            Console.Write($"Enter new value Coordinates of calibration points X for high-pass filter (0...255) ({XHPFIRFiValue}): ");
                            int.TryParse(Console.ReadLine(), out newXHPFIRFi);
                        }
                        else if (line.StartsWith("YHPFIRFi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int YHPFIRFiValue);
                            Console.Write($"Enter new value Coordinates of calibration points Y for high-pass filter (1...100000000000 Hz) ({YHPFIRFiValue}): ");
                            int.TryParse(Console.ReadLine(), out newYHPFIRFi);
                        }
                        else if (line.StartsWith("LPFIRF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int LPFIRFValue);
                            Console.Write($"Enter new value Cutoff frequency of the low-pass filter (1...100000000000 Hz) ({LPFIRFValue}): ");
                            int.TryParse(Console.ReadLine(), out newLPFIRF);
                        }
                        else if (line.StartsWith("NLPFIRF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int NLPFIRFValue);
                            Console.Write($"Enter new value Number of calibration points for cutoff frequency of the low-pass filter (2...100) ({NLPFIRFValue}): ");
                            int.TryParse(Console.ReadLine(), out newNLPFIRF);
                        }
                        else if (line.StartsWith("XLPFIRFi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int XLPFIRFiValue);
                            Console.Write($"Enter new value Coordinates of calibration points X for low-pass filter (0...255) ({XLPFIRFiValue}): ");
                            int.TryParse(Console.ReadLine(), out newXLPFIRFi);
                        }
                        else if (line.StartsWith("YLPFIRFi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int YLPFIRFiValue);
                            Console.Write($"Enter new value Coordinates of calibration points Y for high-pass filter (1...100000000000 Hz) ({YLPFIRFiValue}): ");
                            int.TryParse(Console.ReadLine(), out newYLPFIRFi);
                        }
                        else if (line.StartsWith("BPFIRF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int BPFIRFValue);
                            Console.Write($"Enter new value Center frequency of the 5-band bandpass filter (1...100000000000 Hz) ({BPFIRFValue}): ");
                            int.TryParse(Console.ReadLine(), out newBPFIRF);
                        }
                        else if (line.StartsWith("NBPFIRF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int NBPFIRFValue);
                            Console.Write($"Enter new value Number of calibration points for the center frequency of the 5-band bandpass filter (2...100) ({NBPFIRFValue}): ");
                            int.TryParse(Console.ReadLine(), out newNBPFIRF);
                        }
                        else if (line.StartsWith("XBPFIRFi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int XBPFIRFiValue);
                            Console.Write($"Enter new value Coordinates of calibration points X for 5-band bandpass filter (0...255) ({XBPFIRFiValue}): ");
                            int.TryParse(Console.ReadLine(), out newXBPFIRFi);
                        }
                        else if (line.StartsWith("YBPFIRFi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int YBPFIRFiValue);
                            Console.Write($"Enter new value Coordinates of calibration points Y for 5-band bandpass filter (1...100000000000 Hz) ({YBPFIRFiValue}): ");
                            int.TryParse(Console.ReadLine(), out newYBPFIRFi);
                        }
                        else if (line.StartsWith("FS"))
                        {
                            string[] parts = line.Split('=');
                            newFS = parts[1];

                            // Показ варіантів та вибір нового значення FS
                            Console.WriteLine("Choose value Frequency synthesizer:");
                            Console.WriteLine("1 - hardware_synthesizer");
                            Console.WriteLine("2 - integral_software-hardware_synthesizer");
                            Console.WriteLine("3 - software_synthesizer");

                            Console.Write($"Enter new value Frequency synthesizer ({newFS}): ");
                            int FS = int.Parse(Console.ReadLine());

                            switch (FS)
                            {
                                case 1:
                                    newFS = "hardware_synthesizer";
                                    break;
                                case 2:
                                    newFS = "integral_software-hardware_synthesizer";
                                    break;
                                case 3:
                                    newFS = "software_synthesizer";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default value hardware_synthesizer.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("OFFS1"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int OFFS1Value);
                            Console.Write($"Enter new value Operating frequency of the frequency synthesizer 1 (1...100000000000 Hz) ({OFFS1Value}): ");
                            int.TryParse(Console.ReadLine(), out newOFFS1);
                        }
                        else if (line.StartsWith("NOFFS1"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int NOFFS1Value);
                            Console.Write($"Enter new value Number of calibration points for the operating frequency of the frequency synthesizer 1 (2...100) ({NOFFS1Value}): ");
                            int.TryParse(Console.ReadLine(), out newNOFFS1);
                        }
                        else if (line.StartsWith("XOFFS1i"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int XOFFS1iValue);
                            Console.Write($"Enter new value Coordinates of calibration points X for frequency synthesizer 1 (0...255) ({XOFFS1iValue}): ");
                            int.TryParse(Console.ReadLine(), out newXOFFS1i);
                        }
                        else if (line.StartsWith("YOFFS1i"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int YOFFS1iValue);
                            Console.Write($"Enter new value Coordinates of calibration points Y for frequency synthesizer 1 (1...100000000000 Hz) ({YOFFS1iValue}): ");
                            int.TryParse(Console.ReadLine(), out newYOFFS1i);
                        }
                        else if (line.StartsWith("PLL "))
                        {
                            string[] parts = line.Split('=');
                            newPLL = parts[1];

                            // Показ варіантів та вибір нового значення PLL
                            Console.WriteLine("Choose PLL Type:");
                            Console.WriteLine("1 - hardware_PLL");
                            Console.WriteLine("2 - integrated_software-hardware_PLL");
                            Console.WriteLine("3 - data_PLL");
                            Console.WriteLine("4 - software_PLL");

                            Console.Write($"Enter new PLL Type ({newPLL}): ");
                            string[] PLLs = Console.ReadLine().Split(',');
                            newPLL = "";

                            foreach (var PLL in PLLs)
                            {
                                switch (PLL.Trim())
                                {
                                    case "1":
                                        newPLL += "hardware_PLL";
                                        break;
                                    case "2":
                                        newPLL += "integrated_software-hardware_PLL";
                                        break;
                                    case "3":
                                        newPLL += "data_PLL";
                                        break;
                                    case "4":
                                        newPLL += "software_PLL";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default hardware_PLL.");
                                        break;
                                }
                            }
                        }
                        else if (line.StartsWith("PLLLPF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int PLLLPFValue);
                            Console.Write($"Enter new value Cutoff frequency of the PLL low-pass filter (1...100000000000 Hz) ({PLLLPFValue}): ");
                            int.TryParse(Console.ReadLine(), out newPLLLPF);
                        }
                        else if (line.StartsWith("NPLLLPF"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int NPLLLPFValue);
                            Console.Write($"Enter new value Number of calibration points for cutoff frequency of the PLL low-pass filter (2...100) ({NPLLLPFValue}): ");
                            int.TryParse(Console.ReadLine(), out newNPLLLPF);
                        }
                        else if (line.StartsWith("XPLLLPFi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int XPLLLPFiValue);
                            Console.Write($"Enter new value Coordinates of calibration points X for PLL low-pass filter (0...255) ({XPLLLPFiValue}): ");
                            int.TryParse(Console.ReadLine(), out newXPLLLPFi);
                        }
                        else if (line.StartsWith("YPLLLPFi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int YPLLLPFiValue);
                            Console.Write($"Enter new value Coordinates of calibration points Y for PLL low-pass filter (1...100000000000 Hz) ({YPLLLPFiValue}): ");
                            int.TryParse(Console.ReadLine(), out newYPLLLPFi);
                        }
                        else if (line.StartsWith("SDTR"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int SDTRValue);
                            Console.Write($"Enter new value Symbolic data transfer rate (1...300000000 Symbps) ({SDTRValue}): ");
                            int.TryParse(Console.ReadLine(), out newSDTR);
                        }
                        else if (line.StartsWith("IFB "))
                        {
                            string[] parts = line.Split('=');
                            newIFB = parts[1];

                            // Показ варіантів та вибір нового значення IFB
                            Console.WriteLine("Choose Intermediate frequency band:");
                            Console.WriteLine("1 - calculated");
                            Console.WriteLine("2 - set manually");

                            Console.Write($"Enter new value Intermediate frequency band ({newIFB}): ");
                            int IFB = int.Parse(Console.ReadLine());

                            switch (IFB)
                            {
                                case 1:
                                    newIFB = "calculated";
                                    break;
                                case 2:
                                    newIFB = "set manually";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default calculated.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("SF1"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int SF1Value);
                            Console.Write($"Enter new value Stock factor 1 (0...100) ({SF1Value}): ");
                            int.TryParse(Console.ReadLine(), out newSF1);
                        }
                        else if (line.StartsWith("IFBLPF"))
                        {
                            if (newIFB == "set manually")
                            {
                                string[] parts = line.Split('=');
                                double.TryParse(parts[1], out double IFBLPFValue);
                                Console.Write($"Enter new value Cutoff frequency of the IFB low-pass filter (1...600000000 Hz) ({IFBLPFValue}): ");
                                double.TryParse(Console.ReadLine(), out newIFBLPF);
                            }
                            else if (newIFB == "calculated")
                            {
                                newIFBLPF = (double)(newSDTR * newSF1) / 10.0;
                            }
                        }
                        else if (line.StartsWith("DE"))
                        {
                            string[] parts = line.Split('=');
                            newDE = parts[1];

                            // Показ варіантів та вибір нового значення DE
                            Console.WriteLine("Choose value Data encryption:");
                            Console.WriteLine("1 - data_encryption_is_using");
                            Console.WriteLine("2 - data_encryption_is_not_using");
                            
                            Console.Write($"Enter new value Data encryption ({newDE}): ");
                            int DE = int.Parse(Console.ReadLine());

                            switch (DE)
                            {
                                case 1:
                                    newDE = "data_encryption_is_using";
                                    break;
                                case 2:
                                    newDE = "data_encryption_is_not_using";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default value data_encryption_is_using.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("TE"))
                        {
                            string[] parts = line.Split('=');
                            newTE = parts[1];

                            // Показ варіантів та вибір нового значення TE
                            Console.WriteLine("Choose value Type of encryption:");
                            Console.WriteLine("1 - Type_1");
                            Console.WriteLine("2 - Type_2");

                            Console.Write($"Enter new value Data encryption ({newTE}): ");
                            int TE = int.Parse(Console.ReadLine());

                            switch (TE)
                            {
                                case 1:
                                    newTE = "Type_1";
                                    break;
                                case 2:
                                    newTE = "Type_2";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default value Type_1.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("KE"))
                        {
                            string[] parts = line.Split('=');
                            newKE = parts[1];

                            // Показ варіантів та вибір нового значення KE
                            Console.WriteLine("Choose value Key exchange:");
                            Console.WriteLine("1 - key_exchange_using");
                            Console.WriteLine("2 - key_exchange_not_using");

                            Console.Write($"Enter new value Data encryption ({newKE}): ");
                            int KE = int.Parse(Console.ReadLine());

                            switch (KE)
                            {
                                case 1:
                                    newKE = "key_exchange_using";
                                    break;
                                case 2:
                                    newKE = "key_exchange_not_using";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default value key_exchange_using.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("EDK"))
                        {
                            string[] parts = line.Split('=');
                            string EDKValue = parts[1].Trim();
                            Console.Write($"Enter new value Encryption/decryption key (The key is up to 14 characters long) ({EDKValue}): ");
                            newEDK = Console.ReadLine();
                        }
                        else if (line.StartsWith("CC"))
                        {
                            string[] parts = line.Split('=');
                            newCC = parts[1];

                            // Показ варіантів та вибір нового значення CC
                            Console.WriteLine("Choose value Corrective coding:");
                            Console.WriteLine("1 - corrective_coding_is_using");
                            Console.WriteLine("2 - corrective_coding_is_not_using");

                            Console.Write($"Enter new value Corrective coding ({newCC}): ");
                            int CC = int.Parse(Console.ReadLine());

                            switch (CC)
                            {
                                case 1:
                                    newCC = "corrective_coding_is_using";
                                    break;
                                case 2:
                                    newCC = "corrective_coding_is_not_using";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default value corrective_coding_is_using.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("TCC"))
                        {
                            string[] parts = line.Split('=');
                            newTCC = parts[1];

                            // Показ варіантів та вибір нового значення TCC
                            Console.WriteLine("Choose value Type of corrective coding:");
                            Console.WriteLine("1 - Type_1");
                            Console.WriteLine("2 - Type_2");

                            Console.Write($"Enter new value Type of corrective coding ({newTCC}): ");
                            int TCC = int.Parse(Console.ReadLine());

                            switch (TCC)
                            {
                                case 1:
                                    newTCC = "Type_1";
                                    break;
                                case 2:
                                    newTCC = "Type_2";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default value Type_1.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("SM"))
                        {
                            string[] parts = line.Split('=');
                            newSM = parts[1];

                            // Показ варіантів та вибір нового значення SM
                            Console.WriteLine("Choose value Signal modulation:");
                            Console.WriteLine("1 - ASK");
                            Console.WriteLine("2 - PSK");
                            Console.WriteLine("3 - QAM");
                            Console.WriteLine("4 - AMMC");
                            Console.WriteLine("5 - APK_or_AMMC_special");

                            Console.Write($"Enter new value Signal modulation ({newSM}): ");
                            int SM = int.Parse(Console.ReadLine());

                            switch (SM)
                            {
                                case 1:
                                    newSM = "ASK";
                                    break;
                                case 2:
                                    newSM = "PSK";
                                    break;
                                case 3:
                                    newSM = "QAM";
                                    break;
                                case 4:
                                    newSM = "AMMC";
                                    break;
                                case 5:
                                    newSM = "APK_or_AMMC_special";
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Using default value ASK.");
                                    break;
                            }
                        }
                        else if (line.StartsWith("WC"))
                        {
                            string[] parts = line.Split('=');
                            newWC = parts[1];

                            // Показ варіантів та вибір нового значення WC
                            Console.WriteLine("Choose Working channels:");
                            Console.WriteLine("1 - channel_1");
                            Console.WriteLine("2 - channel_2");
                            Console.WriteLine("3 - channel_3");
                            Console.WriteLine("4 - channel_4");

                            Console.Write($"Enter new Working channels ({newWC}): ");
                            string[] WCs = Console.ReadLine().Split(',');
                            newWC = "";


                            foreach (var WC in WCs)
                            {
                                switch (WC.Trim())
                                {
                                    case "1":
                                        newWC += "channel_1";
                                        break;
                                    case "2":
                                        newWC += "channel_2";
                                        break;
                                    case "3":
                                        newWC += "channel_3";
                                        break;
                                    case "4":
                                        newWC += "channel_4";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default channel_1.");
                                        break;
                                }
                            }
                        }
                        else if (line.StartsWith("ENPS"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int ENPSValue);
                            Console.Write($"Enter new value Effective number of possible symbols (1...4096) ({ENPSValue}): ");
                            int.TryParse(Console.ReadLine(), out newENPS);
                        }
                        else if (line.StartsWith("NSC"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int NSCValue);
                            Console.Write($"Enter new value Number of signal components (1...100) ({NSCValue}): ");
                            int.TryParse(Console.ReadLine(), out newNSC);
                        }
                        else if (line.StartsWith("XSCPi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int XSCPiValue);
                            Console.Write($"Enter new value The coordinates X of the signal constellation points corresponding to the effective number of possible symbols (0...1 with an accuracy of 0.0001) ({XSCPiValue}): ");
                            int.TryParse(Console.ReadLine(), out newXSCPi);
                        }
                        else if (line.StartsWith("YSCPi"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int YSCPiValue);
                            Console.Write($"Enter new value The coordinates Y of the signal constellation points corresponding to the effective number of possible symbols (0...1 with an accuracy of 0.0001) ({YSCPiValue}): ");
                            int.TryParse(Console.ReadLine(), out newYSCPi);
                        }
                        else if (line.StartsWith("HPS"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int HPSValue);
                            Console.Write($"Enter new value Header for phase synchronization (Representation in binary, decimal or hexadecimal number system) ({HPSValue}): ");
                            int.TryParse(Console.ReadLine(), out newHPS);
                        }
                        else if (line.StartsWith("HCS"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int HCSValue);
                            Console.Write($"Enter new value Header for clock synchronization (Representation in binary, decimal or hexadecimal number system) ({HCSValue}): ");
                            int.TryParse(Console.ReadLine(), out newHCS);
                        }
                        else if (line.StartsWith("HFS"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int HFSValue);
                            Console.Write($"Enter new value Header for frame synchronization (Representation in binary, decimal or hexadecimal number system) ({HFSValue}): ");
                            int.TryParse(Console.ReadLine(), out newHFS);
                        }
                        else if (line.StartsWith("DBL"))
                        {
                            string[] parts = line.Split('=');
                            int.TryParse(parts[1], out int DBLValue);
                            Console.Write($"Enter new value Data block length (1...10000000 bit) ({DBLValue}): ");
                            int.TryParse(Console.ReadLine(), out newDBL);
                        }
                        else if (line.StartsWith("FL"))
                        {
                            newFL = newHPS + newHCS + newHFS + newDBL;
                        }
                    }

                    // Запис нових значень параметрів до файлу конфігурації
                    string[] newLines = new string[]
                    {
                            $"[MAIN]",
                            $"TR = {newTR}",
                            $"ORF = {newORF}",
                            $"OFB = {newOFB}",
                            $"UC = {newUC}",
                            $"LOFC = {newLOFC}",
                            $"SU = {newSU}",
                            $"IRF = {newIRF}",
                            $"IRFA = {newIRFA}",
                            $"NIRFA = {newNIRFA}",
                            $"ХIRFAi = {newХIRFAi}",
                            $"YIRFAi = {newYIRFAi}",
                            $"[HPF_PARAMETRS]",
                            $"HPFIRF = {newHPFIRF}",
                            $"NHPFIRF = {newNHPFIRF}",
                            $"XHPFIRFi = {newXHPFIRFi}",
                            $"YHPFIRFi = {newYHPFIRFi}",
                            $"[LPF_PARAMETRS]",
                            $"LPFIRF = {newLPFIRF}",
                            $"NLPFIRF = {newNLPFIRF}",
                            $"XLPFIRFi = {newXLPFIRFi}",
                            $"YLPFIRFi = {newYLPFIRFi}",
                            $"[BPF_PARAMETRS]",
                            $"BPFIRF = {newBPFIRF}",
                            $"NBPFIRF = {newNBPFIRF}",
                            $"XBPFIRFi = {newXBPFIRFi}",
                            $"YBPFIRFi = {newYBPFIRFi}",
                            $"[FS_PARAMETRS]",
                            $"FS = {newFS}",
                            $"OFFS1 = {newOFFS1}",
                            $"NOFFS1 = {newNOFFS1}",
                            $"XOFFS1i = {newXOFFS1i}",
                            $"YOFFS1i = {newYOFFS1i}",
                            $"[PLL_PARAMETRS]",
                            $"PLL = {newPLL}",
                            $"PLLLPF = {newPLLLPF}",
                            $"NPLLLPF = {newNPLLLPF}",
                            $"XPLLLPFi = {newXPLLLPFi}",
                            $"YPLLLPFi = {newYPLLLPFi}",
                            $"[ENC_PARAMETRS]",
                            $"SDTR = {newSDTR}",
                            $"IFB = {newIFB}",
                            $"SF1 = {newSF1}",
                            $"IFBLPF = {newIFBLPF}",
                            $"DE = {newDE}",
                            $"TE = {newTE}",
                            $"KE = {newKE}",
                            $"EDK = {newEDK}",
                            $"CC = {newCC}",
                            $"TCC = {newTCC}",
                            $"SM = {newSM}",
                            $"WC = {newWC}",
                            $"ENPS = {newENPS}",
                            $"NSC = {newNSC}",
                            $"XSCPi = {newXSCPi}",
                            $"YSCPi = {newYSCPi}",
                            $"HPS = {newHPS}",
                            $"HCS = {newHCS}",
                            $"HFS = {newHFS}",
                            $"DBL = {newDBL}",
                            $"FL = {newFL}",
                    };
                    File.WriteAllLines(filePath, newLines);

                    Console.WriteLine("Config updated successfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        private void ResetConfig()
        {
            string filePath = _configuration.GetValue<string>("SettingsFileFilePath"); // змінна для збереження шляху до файлу конфігурації
            string defaultParams = "[MAIN]\nTR = transmitter\nORF = 1\nOFB = operating_frequency_range_1\nUC = converter_is_not_using\nLOFC = 1\nSU = up_sideband\nIRF = 1\nIRFA = 2\nNIRFA = 2\nХIRFAi = 0\nYIRFAi = 2\n[HPF_PARAMETRS]\nHPFIRF = 1\nNHPFIRF = 2\nXHPFIRFi = 0\nYHPFIRFi = 1\n[LPF_PARAMETRS]\nLPFIRF = 1\nNLPFIRF = 2\nXLPFIRFi = 0\nYLPFIRFi = 1\n[BPF_PARAMETRS]\nBPFIRF = 1\nNBPFIRF = 2\nXBPFIRFi = 0\nYBPFIRFi = 1\n[FS_PARAMETRS]\nFS = hardware_synthesizer\nOFFS1 = 1\nNOFFS1 = 2\nXOFFS1i = 0\nYOFFS1i = 1\n[PLL_PARAMETRS]\nPLL = hardware_PLL\nPLLLPF = 1\nNPLLLPF = 2\nXPLLLPFi = 0\nYPLLLPFi = 1\n[ENC_PARAMETRS]\nSDTR = 1\nIFB = calculated\nSF1 = 1\nIFBLPF = 1\nDE = data_encryption_is_using\nTE = Type_1\nKE = key_exchange_using\nEDK = 1\nCC = corrective_coding_is_using\nTCC = Type_1\nSM = ASK\nWC = channel_1\nENPS = 1\nNSC = 1\nXSCPi = 1\nYSCPi = 1\nHPS = 1\nHCS = 1\nHFS = 1\nDBL = 1\nFL = 0"; // значення параметрів за замовчуванням

            try
            {
                // Запис значень параметрів за замовчуванням до файлу конфігурації
                File.WriteAllText(filePath, defaultParams);

                Console.WriteLine("Config reset successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        public void CreateConfig()
        {
            Console.WriteLine("Enter config name:");
            string fileName = Console.ReadLine();

            // Створити директорію, якщо вона не існує
            string directoryPath = _configuration.GetValue<string>("ConfigFolderPath");
            Directory.CreateDirectory(directoryPath);

            // Створити файл конфігурації, якщо він не існує
            string filePath = Path.Combine(directoryPath, fileName + ".cfg");
            if (!File.Exists(filePath))
            {
                string defaultParams = "[MAIN]\nTR = transmitter\nORF = 1\nOFB = operating_frequency_range_1\nUC = converter_is_not_using\nLOFC = 1\nSU = up_sideband\nIRF = 1\nIRFA = 2\nNIRFA = 2\nХIRFAi = 0\nYIRFAi = 2\n[HPF_PARAMETRS]\nHPFIRF = 1\nNHPFIRF = 2\nXHPFIRFi = 0\nYHPFIRFi = 1\n[LPF_PARAMETRS]\nLPFIRF = 1\nNLPFIRF = 2\nXLPFIRFi = 0\nYLPFIRFi = 1\n[BPF_PARAMETRS]\nBPFIRF = 1\nNBPFIRF = 2\nXBPFIRFi = 0\nYBPFIRFi = 1\n[FS_PARAMETRS]\nFS = hardware_synthesizer\nOFFS1 = 1\nNOFFS1 = 2\nXOFFS1i = 0\nYOFFS1i = 1\n[PLL_PARAMETRS]\nPLL = hardware_PLL\nPLLLPF = 1\nNPLLLPF = 2\nXPLLLPFi = 0\nYPLLLPFi = 1\n[ENC_PARAMETRS]\nSDTR = 1\nIFB = calculated\nSF1 = 1\nIFBLPF = 1\nDE = data_encryption_is_using\nTE = Type_1\nKE = key_exchange_using\nEDK = 1\nCC = corrective_coding_is_using\nTCC = Type_1\nSM = ASK\nWC = channel_1\nENPS = 1\nNSC = 1\nXSCPi = 1\nYSCPi = 1\nHPS = 1\nHCS = 1\nHFS = 1\nDBL = 1\nFL = 0"; // значення параметрів за замовчуванням
                File.WriteAllText(filePath, defaultParams);
                string newTR = "transmitter";
                int newORF = 1;
                string newOFB = "operating_frequency_range_1";
                string newUC = "converter_is_not_using";
                int newLOFC = 1;
                string newSU = "up_sideband";
                int newIRF = 1;
                int newIRFA = 2;
                int newNIRFA = 2;
                int newХIRFAi = 0;
                int newYIRFAi = 2;
                int newHPFIRF = 1;
                int newNHPFIRF = 2;
                int newXHPFIRFi = 0;
                int newYHPFIRFi = 1;
                int newLPFIRF = 1;
                int newNLPFIRF = 2;
                int newXLPFIRFi = 0;
                int newYLPFIRFi = 1;
                int newBPFIRF = 1;
                int newNBPFIRF = 2;
                int newXBPFIRFi = 0;
                int newYBPFIRFi = 1;
                string newFS = "hardware_synthesizer";
                int newOFFS1 = 1;
                int newNOFFS1 = 2;
                int newXOFFS1i = 0;
                int newYOFFS1i = 1;
                string newPLL = "hardware_PLL";
                int newPLLLPF = 1;
                int newNPLLLPF = 2;
                int newXPLLLPFi = 0;
                int newYPLLLPFi = 1;
                int newSDTR = 1;
                string newIFB = "calculated";
                int newSF1 = 1;
                double newIFBLPF = 1;
                string newDE = "data_encryption_is_using";
                string newTE = "Type_1";
                string newKE = "key_exchange_using";
                string newEDK = "1";
                string newCC = "corrective_coding_is_using";
                string newTCC = "Type_1";
                string newSM = "ASK";
                string newWC = "channel_1";
                int newENPS = 1;
                int newNSC = 1;
                int newXSCPi = 1;
                int newYSCPi = 1;
                int newHPS = 1;
                int newHCS = 1;
                int newHFS = 1;
                int newDBL = 1;
                int newFL = 0;

                // Перевірка, чи файл існує перед спробою зчитування
                if (File.Exists(filePath))
                {
                    try
                    {
                        // Зчитування поточних значень параметрів
                        string[] lines = File.ReadAllLines(filePath);

                        foreach (string line in lines)
                        {
                            if (line.StartsWith("TR"))
                            {
                                string[] parts = line.Split('=');
                                newTR = parts[1];

                                // Показ варіантів та вибір нового значення TR
                                Console.WriteLine("Choose Transmitter or Receiver:");
                                Console.WriteLine("1 - transmitter");
                                Console.WriteLine("2 - receiver");

                                Console.Write($"Enter new value Transmitter or Receiver ({newTR}): ");
                                int TR = int.Parse(Console.ReadLine());

                                switch (TR)
                                {
                                    case 1:
                                        newTR = "transmitter";
                                        break;
                                    case 2:
                                        newTR = "receiver";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default transmitter.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("ORF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int ORFValue);
                                Console.Write($"Enter new value Operating radio frequency (1...300000000000 Hz) ({ORFValue}): ");
                                int.TryParse(Console.ReadLine(), out newORF);
                            }
                            else if (line.StartsWith("OFB"))
                            {
                                string[] parts = line.Split('=');
                                newOFB = parts[1];

                                // Показ варіантів та вибір нового значення OFB
                                Console.WriteLine("Choose OFB:");
                                Console.WriteLine("1 - operating_frequency_range_1");
                                Console.WriteLine("2 - operating_frequency_range_2");
                                Console.WriteLine("3 - operating_frequency_range_3");
                                Console.WriteLine("4 - operating_frequency_range_4");
                                Console.WriteLine("5 - operating_frequency_range_5");

                                Console.Write($"Enter new OFB ({newOFB}): ");
                                string[] OFBs = Console.ReadLine().Split(',');
                                newOFB = "";

                                foreach (var OFB in OFBs)
                                {
                                    switch (OFB.Trim())
                                    {
                                        case "1":
                                            newOFB += "operating_frequency_range_1";
                                            break;
                                        case "2":
                                            newOFB += "operating_frequency_range_2";
                                            break;
                                        case "3":
                                            newOFB += "operating_frequency_range_3";
                                            break;
                                        case "4":
                                            newOFB += "operating_frequency_range_4";
                                            break;
                                        case "5":
                                            newOFB += "operating_frequency_range_5";
                                            break;
                                        default:
                                            Console.WriteLine("Invalid choice. Using default operating_frequency_range_1.");
                                            break;
                                    }
                                }
                            }
                            else if (line.StartsWith("UC"))
                            {
                                string[] parts = line.Split('=');
                                newUC = parts[1];

                                // Показ варіантів та вибір нового значення UC
                                Console.WriteLine("Choose value Using converter:");
                                Console.WriteLine("1 - converter_is_not_using");
                                Console.WriteLine("2 - up-converter_is_using");
                                Console.WriteLine("3 - down-converter_is_using");

                                Console.Write($"Enter new value Using converter ({newUC}): ");
                                int UC = int.Parse(Console.ReadLine());

                                switch (UC)
                                {
                                    case 1:
                                        newUC = "converter_is_not_using";
                                        break;
                                    case 2:
                                        newUC = "up-converter_is_using";
                                        break;
                                    case 3:
                                        newUC = "down-converter_is_using";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value converter_is_not_using.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("LOFC"))
                            {
                                if (newUC == "up-converter_is_using" || newUC == "down-converter_is_using")
                                {
                                    string[] parts = line.Split('=');
                                    int.TryParse(parts[1], out int LOFCValue);
                                    Console.Write($"Enter new value local oscillator frequency of the converter (1...300000000000 Hz) ({LOFCValue}): ");
                                    int.TryParse(Console.ReadLine(), out newLOFC);
                                }
                            }
                            else if (line.StartsWith("SU"))
                            {
                                if (newUC == "up-converter_is_using")
                                {
                                    string[] parts = line.Split('=');
                                    newSU = parts[1];

                                    // Показ варіантів та вибір нового значення SU
                                    Console.WriteLine("Choose value Sideband usage:");
                                    Console.WriteLine("1 - up_sideband");
                                    Console.WriteLine("2 - down_sideband");

                                    Console.Write($"Enter new value Sideband usage ({newSU}): ");
                                    int SU = int.Parse(Console.ReadLine());

                                    switch (SU)
                                    {
                                        case 1:
                                            newSU = "up_sideband";
                                            break;
                                        case 2:
                                            newSU = "down_sideband";
                                            break;
                                        default:
                                            Console.WriteLine("Invalid choice. Using default value up_sideband.");
                                            break;
                                    }
                                }
                            }
                            else if (line.StartsWith("IRF"))
                            {
                                if (newUC == "converter_is_not_using")
                                {
                                    newIRF = newORF;
                                }
                                else if (newUC == "up-converter_is_using" && newSU == "up_sideband")
                                {
                                    newIRF = newORF - newLOFC;
                                }
                                else if (newUC == "up-converter_is_using" && newSU == "down_sideband")
                                {
                                    newIRF = newLOFC - newORF;
                                }
                                else if (newUC == "down-converter_is_using")
                                {
                                    newIRF = newORF - newLOFC;
                                }
                            }
                            else if (line.StartsWith("IRFA"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int IRFAValue);
                                Console.Write($"Enter new value Intermediate radio frequency amplification (-100...100 dB) ({IRFAValue}): ");
                                int.TryParse(Console.ReadLine(), out newIRFA);
                            }
                            else if (line.StartsWith("NIRFA"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int NIRFAValue);
                                Console.Write($"Enter new value Number of calibration points for intermediate radio frequency amplification (2...100) ({NIRFAValue}): ");
                                int.TryParse(Console.ReadLine(), out newNIRFA);
                            }
                            else if (line.StartsWith("ХIRFAi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int ХIRFAiValue);
                                Console.Write($"Enter new value Coordinates of calibration points X (0...255) ({ХIRFAiValue}): ");
                                int.TryParse(Console.ReadLine(), out newХIRFAi);
                            }
                            else if (line.StartsWith("YIRFAi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int YIRFAiValue);
                                Console.Write($"Enter new value Coordinates of calibration points Y (-100...100 dB) ({YIRFAiValue}): ");
                                int.TryParse(Console.ReadLine(), out newYIRFAi);
                            }
                            else if (line.StartsWith("HPFIRF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int HPFIRFValue);
                                Console.Write($"Enter new value Cutoff frequency of the high-pass filter (1...100000000000 Hz) ({HPFIRFValue}): ");
                                int.TryParse(Console.ReadLine(), out newHPFIRF);
                            }
                            else if (line.StartsWith("NHPFIRF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int NHPFIRFValue);
                                Console.Write($"Enter new value Number of calibration points for cutoff frequency of the high-pass filter (2...100) ({NHPFIRFValue}): ");
                                int.TryParse(Console.ReadLine(), out newNHPFIRF);
                            }
                            else if (line.StartsWith("XHPFIRFi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int XHPFIRFiValue);
                                Console.Write($"Enter new value Coordinates of calibration points X for high-pass filter (0...255) ({XHPFIRFiValue}): ");
                                int.TryParse(Console.ReadLine(), out newXHPFIRFi);
                            }
                            else if (line.StartsWith("YHPFIRFi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int YHPFIRFiValue);
                                Console.Write($"Enter new value Coordinates of calibration points Y for high-pass filter (1...100000000000 Hz) ({YHPFIRFiValue}): ");
                                int.TryParse(Console.ReadLine(), out newYHPFIRFi);
                            }
                            else if (line.StartsWith("LPFIRF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int LPFIRFValue);
                                Console.Write($"Enter new value Cutoff frequency of the low-pass filter (1...100000000000 Hz) ({LPFIRFValue}): ");
                                int.TryParse(Console.ReadLine(), out newLPFIRF);
                            }
                            else if (line.StartsWith("NLPFIRF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int NLPFIRFValue);
                                Console.Write($"Enter new value Number of calibration points for cutoff frequency of the low-pass filter (2...100) ({NLPFIRFValue}): ");
                                int.TryParse(Console.ReadLine(), out newNLPFIRF);
                            }
                            else if (line.StartsWith("XLPFIRFi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int XLPFIRFiValue);
                                Console.Write($"Enter new value Coordinates of calibration points X for low-pass filter (0...255) ({XLPFIRFiValue}): ");
                                int.TryParse(Console.ReadLine(), out newXLPFIRFi);
                            }
                            else if (line.StartsWith("YLPFIRFi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int YLPFIRFiValue);
                                Console.Write($"Enter new value Coordinates of calibration points Y for high-pass filter (1...100000000000 Hz) ({YLPFIRFiValue}): ");
                                int.TryParse(Console.ReadLine(), out newYLPFIRFi);
                            }
                            else if (line.StartsWith("BPFIRF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int BPFIRFValue);
                                Console.Write($"Enter new value Center frequency of the 5-band bandpass filter (1...100000000000 Hz) ({BPFIRFValue}): ");
                                int.TryParse(Console.ReadLine(), out newBPFIRF);
                            }
                            else if (line.StartsWith("NBPFIRF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int NBPFIRFValue);
                                Console.Write($"Enter new value Number of calibration points for the center frequency of the 5-band bandpass filter (2...100) ({NBPFIRFValue}): ");
                                int.TryParse(Console.ReadLine(), out newNBPFIRF);
                            }
                            else if (line.StartsWith("XBPFIRFi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int XBPFIRFiValue);
                                Console.Write($"Enter new value Coordinates of calibration points X for 5-band bandpass filter (0...255) ({XBPFIRFiValue}): ");
                                int.TryParse(Console.ReadLine(), out newXBPFIRFi);
                            }
                            else if (line.StartsWith("YBPFIRFi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int YBPFIRFiValue);
                                Console.Write($"Enter new value Coordinates of calibration points Y for 5-band bandpass filter (1...100000000000 Hz) ({YBPFIRFiValue}): ");
                                int.TryParse(Console.ReadLine(), out newYBPFIRFi);
                            }
                            else if (line.StartsWith("FS"))
                            {
                                string[] parts = line.Split('=');
                                newFS = parts[1];

                                // Показ варіантів та вибір нового значення FS
                                Console.WriteLine("Choose value Frequency synthesizer:");
                                Console.WriteLine("1 - hardware_synthesizer");
                                Console.WriteLine("2 - integral_software-hardware_synthesizer");
                                Console.WriteLine("3 - software_synthesizer");

                                Console.Write($"Enter new value Frequency synthesizer ({newFS}): ");
                                int FS = int.Parse(Console.ReadLine());

                                switch (FS)
                                {
                                    case 1:
                                        newFS = "hardware_synthesizer";
                                        break;
                                    case 2:
                                        newFS = "integral_software-hardware_synthesizer";
                                        break;
                                    case 3:
                                        newFS = "software_synthesizer";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value hardware_synthesizer.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("OFFS1"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int OFFS1Value);
                                Console.Write($"Enter new value Operating frequency of the frequency synthesizer 1 (1...100000000000 Hz) ({OFFS1Value}): ");
                                int.TryParse(Console.ReadLine(), out newOFFS1);
                            }
                            else if (line.StartsWith("NOFFS1"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int NOFFS1Value);
                                Console.Write($"Enter new value Number of calibration points for the operating frequency of the frequency synthesizer 1 (2...100) ({NOFFS1Value}): ");
                                int.TryParse(Console.ReadLine(), out newNOFFS1);
                            }
                            else if (line.StartsWith("XOFFS1i"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int XOFFS1iValue);
                                Console.Write($"Enter new value Coordinates of calibration points X for frequency synthesizer 1 (0...255) ({XOFFS1iValue}): ");
                                int.TryParse(Console.ReadLine(), out newXOFFS1i);
                            }
                            else if (line.StartsWith("YOFFS1i"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int YOFFS1iValue);
                                Console.Write($"Enter new value Coordinates of calibration points Y for frequency synthesizer 1 (1...100000000000 Hz) ({YOFFS1iValue}): ");
                                int.TryParse(Console.ReadLine(), out newYOFFS1i);
                            }
                            else if (line.StartsWith("PLL "))
                            {
                                string[] parts = line.Split('=');
                                newPLL = parts[1];

                                // Показ варіантів та вибір нового значення PLL
                                Console.WriteLine("Choose PLL Type:");
                                Console.WriteLine("1 - hardware_PLL");
                                Console.WriteLine("2 - integrated_software-hardware_PLL");
                                Console.WriteLine("3 - data_PLL");
                                Console.WriteLine("4 - software_PLL");

                                Console.Write($"Enter new PLL Type ({newPLL}): ");
                                string[] PLLs = Console.ReadLine().Split(',');
                                newPLL = "";

                                foreach (var PLL in PLLs)
                                {
                                    switch (PLL.Trim())
                                    {
                                        case "1":
                                            newPLL += "hardware_PLL";
                                            break;
                                        case "2":
                                            newPLL += "integrated_software-hardware_PLL";
                                            break;
                                        case "3":
                                            newPLL += "data_PLL";
                                            break;
                                        case "4":
                                            newPLL += "software_PLL";
                                            break;
                                        default:
                                            Console.WriteLine("Invalid choice. Using default hardware_PLL.");
                                            break;
                                    }
                                }
                            }
                            else if (line.StartsWith("PLLLPF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int PLLLPFValue);
                                Console.Write($"Enter new value Cutoff frequency of the PLL low-pass filter (1...100000000000 Hz) ({PLLLPFValue}): ");
                                int.TryParse(Console.ReadLine(), out newPLLLPF);
                            }
                            else if (line.StartsWith("NPLLLPF"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int NPLLLPFValue);
                                Console.Write($"Enter new value Number of calibration points for cutoff frequency of the PLL low-pass filter (2...100) ({NPLLLPFValue}): ");
                                int.TryParse(Console.ReadLine(), out newNPLLLPF);
                            }
                            else if (line.StartsWith("XPLLLPFi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int XPLLLPFiValue);
                                Console.Write($"Enter new value Coordinates of calibration points X for PLL low-pass filter (0...255) ({XPLLLPFiValue}): ");
                                int.TryParse(Console.ReadLine(), out newXPLLLPFi);
                            }
                            else if (line.StartsWith("YPLLLPFi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int YPLLLPFiValue);
                                Console.Write($"Enter new value Coordinates of calibration points Y for PLL low-pass filter (1...100000000000 Hz) ({YPLLLPFiValue}): ");
                                int.TryParse(Console.ReadLine(), out newYPLLLPFi);
                            }
                            else if (line.StartsWith("SDTR"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int SDTRValue);
                                Console.Write($"Enter new value Symbolic data transfer rate (1...300000000 Symbps) ({SDTRValue}): ");
                                int.TryParse(Console.ReadLine(), out newSDTR);
                            }
                            else if (line.StartsWith("IFB "))
                            {
                                string[] parts = line.Split('=');
                                newIFB = parts[1];

                                // Показ варіантів та вибір нового значення IFB
                                Console.WriteLine("Choose Intermediate frequency band:");
                                Console.WriteLine("1 - calculated");
                                Console.WriteLine("2 - set manually");

                                Console.Write($"Enter new value Intermediate frequency band ({newIFB}): ");
                                int IFB = int.Parse(Console.ReadLine());

                                switch (IFB)
                                {
                                    case 1:
                                        newIFB = "calculated";
                                        break;
                                    case 2:
                                        newIFB = "set manually";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default calculated.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("SF1"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int SF1Value);
                                Console.Write($"Enter new value Stock factor 1 (0...100) ({SF1Value}): ");
                                int.TryParse(Console.ReadLine(), out newSF1);
                            }
                            else if (line.StartsWith("IFBLPF"))
                            {
                                if (newIFB == "set manually")
                                {
                                    string[] parts = line.Split('=');
                                    double.TryParse(parts[1], out double IFBLPFValue);
                                    Console.Write($"Enter new value Cutoff frequency of the IFB low-pass filter (1...600000000 Hz) ({IFBLPFValue}): ");
                                    double.TryParse(Console.ReadLine(), out newIFBLPF);
                                }
                                else if (newIFB == "calculated")
                                {
                                    newIFBLPF = (double)(newSDTR * newSF1) / 10.0;
                                }
                            }
                            else if (line.StartsWith("DE"))
                            {
                                string[] parts = line.Split('=');
                                newDE = parts[1];

                                // Показ варіантів та вибір нового значення DE
                                Console.WriteLine("Choose value Data encryption:");
                                Console.WriteLine("1 - data_encryption_is_using");
                                Console.WriteLine("2 - data_encryption_is_not_using");

                                Console.Write($"Enter new value Data encryption ({newDE}): ");
                                int DE = int.Parse(Console.ReadLine());

                                switch (DE)
                                {
                                    case 1:
                                        newDE = "data_encryption_is_using";
                                        break;
                                    case 2:
                                        newDE = "data_encryption_is_not_using";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value data_encryption_is_using.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("TE"))
                            {
                                string[] parts = line.Split('=');
                                newTE = parts[1];

                                // Показ варіантів та вибір нового значення TE
                                Console.WriteLine("Choose value Type of encryption:");
                                Console.WriteLine("1 - Type_1");
                                Console.WriteLine("2 - Type_2");

                                Console.Write($"Enter new value Data encryption ({newTE}): ");
                                int TE = int.Parse(Console.ReadLine());

                                switch (TE)
                                {
                                    case 1:
                                        newTE = "Type_1";
                                        break;
                                    case 2:
                                        newTE = "Type_2";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value Type_1.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("KE"))
                            {
                                string[] parts = line.Split('=');
                                newKE = parts[1];

                                // Показ варіантів та вибір нового значення KE
                                Console.WriteLine("Choose value Key exchange:");
                                Console.WriteLine("1 - key_exchange_using");
                                Console.WriteLine("2 - key_exchange_not_using");

                                Console.Write($"Enter new value Data encryption ({newKE}): ");
                                int KE = int.Parse(Console.ReadLine());

                                switch (KE)
                                {
                                    case 1:
                                        newKE = "key_exchange_using";
                                        break;
                                    case 2:
                                        newKE = "key_exchange_not_using";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value key_exchange_using.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("EDK"))
                            {
                                string[] parts = line.Split('=');
                                string EDKValue = parts[1].Trim();
                                Console.Write($"Enter new value Encryption/decryption key (The key is up to 14 characters long) ({EDKValue}): ");
                                newEDK = Console.ReadLine();
                            }
                            else if (line.StartsWith("CC"))
                            {
                                string[] parts = line.Split('=');
                                newCC = parts[1];

                                // Показ варіантів та вибір нового значення CC
                                Console.WriteLine("Choose value Corrective coding:");
                                Console.WriteLine("1 - corrective_coding_is_using");
                                Console.WriteLine("2 - corrective_coding_is_not_using");

                                Console.Write($"Enter new value Corrective coding ({newCC}): ");
                                int CC = int.Parse(Console.ReadLine());

                                switch (CC)
                                {
                                    case 1:
                                        newCC = "corrective_coding_is_using";
                                        break;
                                    case 2:
                                        newCC = "corrective_coding_is_not_using";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value corrective_coding_is_using.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("TCC"))
                            {
                                string[] parts = line.Split('=');
                                newTCC = parts[1];

                                // Показ варіантів та вибір нового значення TCC
                                Console.WriteLine("Choose value Type of corrective coding:");
                                Console.WriteLine("1 - Type_1");
                                Console.WriteLine("2 - Type_2");

                                Console.Write($"Enter new value Type of corrective coding ({newTCC}): ");
                                int TCC = int.Parse(Console.ReadLine());

                                switch (TCC)
                                {
                                    case 1:
                                        newTCC = "Type_1";
                                        break;
                                    case 2:
                                        newTCC = "Type_2";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value Type_1.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("SM"))
                            {
                                string[] parts = line.Split('=');
                                newSM = parts[1];

                                // Показ варіантів та вибір нового значення SM
                                Console.WriteLine("Choose value Signal modulation:");
                                Console.WriteLine("1 - ASK");
                                Console.WriteLine("2 - PSK");
                                Console.WriteLine("3 - QAM");
                                Console.WriteLine("4 - AMMC");
                                Console.WriteLine("5 - APK_or_AMMC_special");

                                Console.Write($"Enter new value Signal modulation ({newSM}): ");
                                int SM = int.Parse(Console.ReadLine());

                                switch (SM)
                                {
                                    case 1:
                                        newSM = "ASK";
                                        break;
                                    case 2:
                                        newSM = "PSK";
                                        break;
                                    case 3:
                                        newSM = "QAM";
                                        break;
                                    case 4:
                                        newSM = "AMMC";
                                        break;
                                    case 5:
                                        newSM = "APK_or_AMMC_special";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Using default value ASK.");
                                        break;
                                }
                            }
                            else if (line.StartsWith("WC"))
                            {
                                string[] parts = line.Split('=');
                                newWC = parts[1];

                                // Показ варіантів та вибір нового значення WC
                                Console.WriteLine("Choose Working channels:");
                                Console.WriteLine("1 - channel_1");
                                Console.WriteLine("2 - channel_2");
                                Console.WriteLine("3 - channel_3");
                                Console.WriteLine("4 - channel_4");

                                Console.Write($"Enter new Working channels ({newWC}): ");
                                string[] WCs = Console.ReadLine().Split(',');
                                newWC = "";


                                foreach (var WC in WCs)
                                {
                                    switch (WC.Trim())
                                    {
                                        case "1":
                                            newWC += "channel_1";
                                            break;
                                        case "2":
                                            newWC += "channel_2";
                                            break;
                                        case "3":
                                            newWC += "channel_3";
                                            break;
                                        case "4":
                                            newWC += "channel_4";
                                            break;
                                        default:
                                            Console.WriteLine("Invalid choice. Using default channel_1.");
                                            break;
                                    }
                                }
                            }
                            else if (line.StartsWith("ENPS"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int ENPSValue);
                                Console.Write($"Enter new value Effective number of possible symbols (1...4096) ({ENPSValue}): ");
                                int.TryParse(Console.ReadLine(), out newENPS);
                            }
                            else if (line.StartsWith("NSC"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int NSCValue);
                                Console.Write($"Enter new value Number of signal components (1...100) ({NSCValue}): ");
                                int.TryParse(Console.ReadLine(), out newNSC);
                            }
                            else if (line.StartsWith("XSCPi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int XSCPiValue);
                                Console.Write($"Enter new value The coordinates X of the signal constellation points corresponding to the effective number of possible symbols (0...1 with an accuracy of 0.0001) ({XSCPiValue}): ");
                                int.TryParse(Console.ReadLine(), out newXSCPi);
                            }
                            else if (line.StartsWith("YSCPi"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int YSCPiValue);
                                Console.Write($"Enter new value The coordinates Y of the signal constellation points corresponding to the effective number of possible symbols (0...1 with an accuracy of 0.0001) ({YSCPiValue}): ");
                                int.TryParse(Console.ReadLine(), out newYSCPi);
                            }
                            else if (line.StartsWith("HPS"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int HPSValue);
                                Console.Write($"Enter new value Header for phase synchronization (Representation in binary, decimal or hexadecimal number system) ({HPSValue}): ");
                                int.TryParse(Console.ReadLine(), out newHPS);
                            }
                            else if (line.StartsWith("HCS"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int HCSValue);
                                Console.Write($"Enter new value Header for clock synchronization (Representation in binary, decimal or hexadecimal number system) ({HCSValue}): ");
                                int.TryParse(Console.ReadLine(), out newHCS);
                            }
                            else if (line.StartsWith("HFS"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int HFSValue);
                                Console.Write($"Enter new value Header for frame synchronization (Representation in binary, decimal or hexadecimal number system) ({HFSValue}): ");
                                int.TryParse(Console.ReadLine(), out newHFS);
                            }
                            else if (line.StartsWith("DBL"))
                            {
                                string[] parts = line.Split('=');
                                int.TryParse(parts[1], out int DBLValue);
                                Console.Write($"Enter new value Data block length (1...10000000 bit) ({DBLValue}): ");
                                int.TryParse(Console.ReadLine(), out newDBL);
                            }
                            else if (line.StartsWith("FL"))
                            {
                                newFL = newHPS + newHCS + newHFS + newDBL;
                            }
                        }

                        // Запис нових значень параметрів до файлу конфігурації
                        string[] newLines = new string[]
                        {
                            $"[MAIN]",
                            $"TR = {newTR}",
                            $"ORF = {newORF}",
                            $"OFB = {newOFB}",
                            $"UC = {newUC}",
                            $"LOFC = {newLOFC}",
                            $"SU = {newSU}",
                            $"IRF = {newIRF}",
                            $"IRFA = {newIRFA}",
                            $"NIRFA = {newNIRFA}",
                            $"ХIRFAi = {newХIRFAi}",
                            $"YIRFAi = {newYIRFAi}",
                            $"[HPF_PARAMETRS]",
                            $"HPFIRF = {newHPFIRF}",
                            $"NHPFIRF = {newNHPFIRF}",
                            $"XHPFIRFi = {newXHPFIRFi}",
                            $"YHPFIRFi = {newYHPFIRFi}",
                            $"[LPF_PARAMETRS]",
                            $"LPFIRF = {newLPFIRF}",
                            $"NLPFIRF = {newNLPFIRF}",
                            $"XLPFIRFi = {newXLPFIRFi}",
                            $"YLPFIRFi = {newYLPFIRFi}",
                            $"[BPF_PARAMETRS]",
                            $"BPFIRF = {newBPFIRF}",
                            $"NBPFIRF = {newNBPFIRF}",
                            $"XBPFIRFi = {newXBPFIRFi}",
                            $"YBPFIRFi = {newYBPFIRFi}",
                            $"[FS_PARAMETRS]",
                            $"FS = {newFS}",
                            $"OFFS1 = {newOFFS1}",
                            $"NOFFS1 = {newNOFFS1}",
                            $"XOFFS1i = {newXOFFS1i}",
                            $"YOFFS1i = {newYOFFS1i}",
                            $"[PLL_PARAMETRS]",
                            $"PLL = {newPLL}",
                            $"PLLLPF = {newPLLLPF}",
                            $"NPLLLPF = {newNPLLLPF}",
                            $"XPLLLPFi = {newXPLLLPFi}",
                            $"YPLLLPFi = {newYPLLLPFi}",
                            $"[ENC_PARAMETRS]",
                            $"SDTR = {newSDTR}",
                            $"IFB = {newIFB}",
                            $"SF1 = {newSF1}",
                            $"IFBLPF = {newIFBLPF}",
                            $"DE = {newDE}",
                            $"TE = {newTE}",
                            $"KE = {newKE}",
                            $"EDK = {newEDK}",
                            $"CC = {newCC}",
                            $"TCC = {newTCC}",
                            $"SM = {newSM}",
                            $"WC = {newWC}",
                            $"ENPS = {newENPS}",
                            $"NSC = {newNSC}",
                            $"XSCPi = {newXSCPi}",
                            $"YSCPi = {newYSCPi}",
                            $"HPS = {newHPS}",
                            $"HCS = {newHCS}",
                            $"HFS = {newHFS}",
                            $"DBL = {newDBL}",
                            $"FL = {newFL}",
                        };
                        File.WriteAllLines(filePath, newLines);

                        Console.WriteLine("Config created successfully.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("File with name {0} already created {1}", fileName + ".cfg", directoryPath);
            }
        }
        public void LoadConfig()
        {
            // Оглянути директорію configs та вивести список файлів конфігурації
            string directoryPath = _configuration.GetValue<string>("ConfigFolderPath");
            string[] configFiles = Directory.GetFiles(directoryPath, "*.cfg");

            // Вивести назви файлів та дату їх створення, пронумерувати їх вибір
            Console.WriteLine("Config files:");
            for (int i = 0; i < configFiles.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(configFiles[i]);
                DateTime createdDate = File.GetCreationTime(configFiles[i]);
                Console.WriteLine($"{i + 1}. {fileName} (created: {createdDate})");
            }

            // Вибрати файл конфігурації та зберегти номер вибору
            Console.Write("Choose config file: ");
            int choice = Convert.ToInt32(Console.ReadLine());

            // Перевірити, що номер вибору відповідає існуючому файлу конфігурації
            if (choice < 1 || choice > configFiles.Length)
            {
                Console.WriteLine("Invalid selection!");
                return;
            }

            // Завантажити вибраний файл конфігурації та виконати необхідні налаштування
            string selectedConfigFile = configFiles[choice - 1];
            // your code here to work with the selected configuration file
        }
    }

       class TelnetServer
        {
            private TcpListener listener;
            private int port = 23; // Telnet port number
            private string pdfFilePath = ""; // Path to PDF file to send

            public TelnetServer(string filePath)
            {
                pdfFilePath = filePath;
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                Console.WriteLine("Telnet server started on port " + port);
            }

            public void SendPdfFile()
            {
                Console.WriteLine("Waiting for client connection...");
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);

                // Read the client's request
                string request = reader.ReadLine();

                if (request == "send pdf")
                {
                    // Send the PDF file to the client
                    Console.WriteLine("Sending PDF file...");
                    FileStream fileStream = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[100000000];
                    int bytesRead = 0;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, bytesRead);
                    }

                    fileStream.Close();
                }

                writer.Close();
                reader.Close();
                stream.Close();
                client.Close();

                Console.WriteLine("PDF file sent.");
            }
        }

       class TelnetClient
        {
            public TcpClient client;
            private string serverAddress = "localhost"; // Server address
            private int serverPort = 23; // Server port number

            public TelnetClient()
            {
                client = new TcpClient(serverAddress, serverPort);

                Console.WriteLine("Connected to Telnet server.");
            }
            public void SavePdfFile(string filePath)
            {
                Console.WriteLine("Saving PDF file...");

                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);

                // Send the request to the server
                writer.WriteLine("send pdf");
                writer.Flush();

                // Read the PDF file from the server
                FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                byte[] buffer = new byte[100000000];
                int bytesRead = 0;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                }

                fileStream.Close();

                writer.Close();
                reader.Close();
                stream.Close();
                client.Close();

                Console.WriteLine("PDF file saved.");
            }
        }

    static class CredentialService
    {
        public static void UpdateCredentials(string filePath, List<User> users)
        {
            foreach (var item in users)
            {
                item.Login = EncodeToBase64(item.Login);
                item.Password = EncodeToBase64(item.Password);
            }

            var stringContent = JsonConvert.SerializeObject(users, Formatting.Indented);

            File.WriteAllText(filePath, stringContent);
        }

        public static List<User> GetUsersFromConfigFile(string filePath)
        {
            var users = GetEncryptedUsersFromConfigFile(filePath);

            foreach (var item in users)
            {
                item.Login = DecodeFrom64(item.Login);
                item.Password = DecodeFrom64(item.Password);
            }

            return users;
        }

        private static List<User> GetEncryptedUsersFromConfigFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var stingContent = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<User>>(stingContent);
            }
            else
            {
                return new List<User>();
            }
        }

        public static string EncodeToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        private static string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new(decoded_char);
            return result;
        }

    }
}
