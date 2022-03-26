
using System.Text.RegularExpressions;

namespace puic {
    public class Program {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to PUIC Holidays!");
            Console.ResetColor();

            // try logging in
            var loop = false;
            Console.WriteLine("Would you like to search for a holiday?");
            var cont = Console.ReadLine();
            try {
                var shouldCont = CheckContinue(cont ?? "");
                if (!shouldCont) {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                } else {
                    do {
                        var loggedIn = Login();
                        if (loggedIn) { //successfully logged in
                            loop = false;
                        }
                        else {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Wrong username or password, check the format");
                            Console.ResetColor();
                            loop = true;
                            continue;
                        }
                    } while (loop);
                }
            }
            catch (ArgumentException e) {
                Console.WriteLine($"Error: {e.Message}");
            }

            var dests =  new List<string>(){"NIGERIA", "KENYA", "GREECE", "ARGENTINA", "CANADA", "ISRAEL", "MEXICO", "GERMANY", "JAPAN", "ITALY", "PORTUGAL", "PAIN", "FRANCE", "INDIA", "UKRAINE", "POLAND", "BRITAIN", "SOUTH AFRICA", "PAKISTAN"};
            Console.WriteLine("Would you like to do a Search or Choose from a list of destinations?");
            int optionInt;
            var optionAgain = false;
            do {
                Console.WriteLine("If Search, input '1', else input '2'");
                var option = Console.ReadLine();
                
                var optionParsed = Int32.TryParse(option, out optionInt);
                if (!optionParsed || optionInt > 2 || optionInt <1) { 
                    Console.ForegroundColor = ConsoleColor.Yellow;   
                    Console.WriteLine("You either chose a non-digit option or a number out of {1,2}");
                    Console.ResetColor();
                    optionAgain = true;
                } else {
                    optionAgain = false;
                }
            } while (optionAgain);
            
            string chosen = "";
            if (optionInt == 1) { // User chose to search 
                var searchDestAgain = false;
                int searchPos;
                do {
                    Console.WriteLine("\nInput the destination you want to search");
                    var searchInput = Normalize(Console.ReadLine() ?? "");
                    searchPos = dests.IndexOf(searchInput);

                    if (searchPos == -1) { // search strig not in the destinations list
                        Console.ForegroundColor = ConsoleColor.Yellow;   
                        Console.WriteLine("We're so sorry. Your distination is not on the list");
                        Console.ResetColor();
                        searchDestAgain = true;
                    } else {
                        searchDestAgain = false;
                        chosen = dests[searchPos];
                    }
                } while (searchDestAgain);
            } else if (optionInt == 2) { //User chose to get the list and select from there
                Console.WriteLine("\nChoose from the following destinations:");
                var count = 1;
                foreach (var dest in dests) // print out all the destinations
                {
                    Console.WriteLine($"{count}. {dest}");
                    count++;
                }
                var numberAgain = false;
                do {
                    Console.WriteLine("\nGive the number that corresponds to the destination of your choice");
                    int choice = 0;
                    try {
                        var parsed = Int32.TryParse(Console.ReadLine(), out choice);
                        Console.WriteLine(choice);
                    }
                    catch (Exception e) {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                    if (choice < 1 || choice > dests.Count()) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("You didn't quite get that right!, Your choice is out of bounds. Try again");
                        Console.ResetColor();
                        numberAgain = true;
                    } else {
                        numberAgain = false;
                        chosen = dests[choice-1];
                    } 
                } while (numberAgain);
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nYou have selected {chosen}. Now choose the dates");
            Console.ResetColor();
            var dateAgain = false;
            string? beginStr = "";
            DateFormat beginFormat;
            DateOnly beginDate;
            do {
                Console.WriteLine("\nChoose beginning date in the following format: 'dd/mm/yyyy' or 'dd-mm-yyyy'");
                beginStr = Console.ReadLine();
                beginFormat = GetDateFormat(beginStr ?? "");
                if (beginFormat == DateFormat.Invalid) {
                    Console.WriteLine("Invalid Date selected, please try again");
                    dateAgain = true;
                } else {
                    dateAgain = false;
                    try {
                        beginDate = ParseDate(beginStr ?? "", beginFormat);

                    }
                    catch {
                        dateAgain = true;
                        continue;
                    }
                }
            } while (dateAgain);

            var dateAgain2 = false;
            string? endStr = "";
            DateFormat endFormat;
            DateOnly endDate;
            do {
                Console.WriteLine("\nChoose ending date in the following format: 'dd/mm/yyyy' or 'dd-mm-yyyy'");
                endStr = Console.ReadLine();
                endFormat = GetDateFormat(endStr ?? "");
                if (endFormat == DateFormat.Invalid) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Invalid Date selected, please try again");
                    Console.ResetColor();
                    dateAgain2 = true;
                } else {
                    dateAgain2 = false;
                    try {
                        endDate = ParseDate(endStr ?? "", endFormat);
                    }
                    catch {
                        dateAgain2 = true;
                        continue;
                    }
                }
            } while (dateAgain2);
            
            Package package;
            try {
                package = GetPackage(beginDate, endDate);
                if (package == Package.SeniorCitizens) {
                    var ageAgain = false;
                    int age;
                    do {
                        Console.WriteLine("\nPlease input your age");
                        bool parsed = Int32.TryParse(Console.ReadLine(), out age);
                        if (!parsed || age < 0) {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\nInvalid input");
                            Console.ResetColor();
                            ageAgain = true;
                        } else {
                            ageAgain = false;
                        }
                    } while(ageAgain);
                    
                    if (age <= 60) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nYou are not qualified for this package \nGoodbye!");
                        Environment.Exit(0);
                    }
                }
                int cost = Cost(package);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nYou have successfully reserved a {PackageName(package)} spot at PUIC Holidays. \nYour destination is {chosen} \nMake sure you pay the stipulated sum of £{cost} within 7 days");
                Environment.Exit(0);
            }
            catch (Exception e) {
                    Console.WriteLine($"Error: {e.Message}");
            }

            
        }

        // PackageName returns the name of the Package based on the package type, e.g. "Mini"
        static string PackageName(Package p) {
            string packageName;
            switch (p)
            {
                case Package.Mini:
                    packageName = "Mini";
                    break;
                case Package.AllIclusive:
                    packageName = "All-Iclusive";
                    break;
                case Package.SeniorCitizens:
                    packageName = "Senior-Citizens";
                    break;
                default: 
                    packageName = "Invalid";
                    break;
            }
            return packageName;
        }

        // GetPackage gives the Package enum option based on the date intervals. throws exception if the date is invalid 
        static Package GetPackage(DateOnly begin, DateOnly end) {
            Package package;
            switch (end.DayNumber - begin.DayNumber)
            {
                case 3:
                    package = Package.Mini;
                    break;
                case 7:
                    package = Package.AllIclusive;
                    break;
                case 14: 
                    package = Package.SeniorCitizens;
                    break;
                default:
                package = Package.Invalid;
                    throw new Exception("\nInvalid Package Choice. This is because you didn't choose your dates well.");
                         
            }
            return package;
        }
        
        // Cost determines the cost of the package based on the Package type
        static int Cost (Package p)
        {
            int c;
            switch (p)
            {
                case Package.Mini:
                    c = 199;
                    break;
                case Package.AllIclusive:
                    c = 599;
                    break;
                case Package.SeniorCitizens:
                    c = 699;
                    break;
                default: //based on how we use the Cost() method, this default will never happen
                    c = 0;
                    break;
            }
            
            return c;
        }    
       
       // GetDateFormat gets the format used by the user to specify their dates, depending on the character used to divide.
       // it can be invalid
        static DateFormat GetDateFormat(string s) {
            s = s.Trim();
            string slashPattern  = @"^\d{2}/\d{2}/\d{4}$";
            string dashPattern = @"^\d{2}-\d{2}-\d{4}$";
            Regex srg = new Regex(slashPattern);
            Regex drg = new Regex(dashPattern);

            if (srg.IsMatch(s)) {
                return DateFormat.Slash;
            } else if (drg.IsMatch(s)) {
                return DateFormat.Dash;
            } else {
                return DateFormat.Invalid;
            }

        }
        
        // ParseDate returns a DateOly object which is the parsed date from given string. we support two date formats
        static DateOnly ParseDate(string s, DateFormat f) {
            DateOnly date;
            if (f == DateFormat.Slash) {
                try{
                    date = DateOnly.ParseExact(s, "dd/mm/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception e)  {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nError: Bad date format. Start again");
                    Console.ResetColor();
                    throw new Exception(e.Message);
                }
                
            } else if (f == DateFormat.Dash) {
                try {
                    date = DateOnly.ParseExact(s, "dd-mm-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception e) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nError: Bad date format. Start again");
                    Console.ResetColor();
                    throw new Exception(e.Message);
                }
            } else {
                throw new ArgumentException("Wrong format. check again");
            }
            return date;
        }
        
        // Login checks the conditions that validate both username and password input.
        // returns a boolean that determines if the user is authorized to continue
        static bool Login() {
            try {
                Console.WriteLine("Enter your UserName: ");
                var uname = Console.ReadLine();
                Console.WriteLine("Enter your Password: ");
                var password = Console.ReadLine();
                var unameValid = false;
                var psswdValid = false;
                if ((Normalize(uname ?? "").Length == 8) && (Normalize(uname ?? "").All(char.IsLetter))) {
                    unameValid = true;
                }
                int psswd;
                if(Normalize(password ?? "").Length == 6 && Int32.TryParse(Normalize(password ?? "") ?? "", out psswd)) {
                    psswdValid = true;
                }

                if (unameValid && psswdValid) {
                    return true;
                } else {
                    return false;
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Error: {e.Message}");
                return false; //comeback
            }
            
        } 
        
        // CheckContinue checks to see if the user wants to continue or not, based on whether thy inputed 'Y' or 'N'
        static bool CheckContinue(string s) {
            string yes = "Y";
            string no = "N";
            if (Normalize(s) == yes) {
                return true;
            } else if (Normalize(s) == no) {
                return false;
            } else {
                throw new ArgumentException("'y' or 'n' expected");
            }
        }
        
        // Normalize trims the string and makes it all capital
        static string Normalize(string s) {
            return s.Trim().ToUpper();
        }
    } 
    enum DateFormat {
        Slash,
        Dash,
        Invalid,
    }

    enum Package
    {
        Mini,
        AllIclusive,
        SeniorCitizens,
        Invalid,
    }

}
