// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;
namespace ATM_Project
{
    class Program
    {
        static readonly string userDataFilePath = "C:\\Users\\user\\Desktop\\ATM\\ATM - project\\Atm.json";
        static UserData? userData;

        static void Main(string[] args)
        {
            // მომხმარებლის მონაცემების ჩატვირთვა JSON ფაილიდან
            userData = LoadUserData(userDataFilePath);
            if (userData == null)
            {
                Console.WriteLine("Failed to load user data. Exiting...");
                return;
            }

            while (true)
            {
                Console.WriteLine("Welcome to the ATM Simulator!");
                Console.WriteLine("Please insert your card.");

                // ბარათის ვერიფიკაცია
                if (!VerifyCard())
                    continue;

                // პინ კოდის ვერიფიკაცია
                if (!VerifyPin())
                    continue;

                bool continueLoop = true;

                while (continueLoop)
                {
                    // მენიუს ჩართვა
                    DisplayMenu();

                    // მომხმარებლის არჩევანის დაფიქსირება
                    string choice = Console.ReadLine();
                    continueLoop = ProcessChoice(choice);

                    // მომხმარებლის მონაცემების შენახვა JSON ფაილში
                    SaveUserData(userData, userDataFilePath);
                }
            }
        }

        static bool VerifyCard()
        {
            Console.Write("Enter card number: ");
            string cardNumber = Console.ReadLine();
            Console.Write("Enter expiration date (MM/YY): ");
            string expirationDate = Console.ReadLine();

            if (cardNumber != userData.CardDetails.CardNumber || expirationDate != userData.CardDetails.ExpirationDate)
            {
                Console.WriteLine("Invalid card information. Please try again.");
                return false;
            }
            return true;
        }

        static bool VerifyPin()
        {
            Console.Write("Enter pin code: ");
            string pinCode = Console.ReadLine();

            if (pinCode != userData.PinCode)
            {
                Console.WriteLine("Incorrect pin code. Please try again.");
                return false;
            }
            return true;
        }

        static void DisplayMenu()
        {
            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. View balance");
            Console.WriteLine("2. Withdraw money");
            Console.WriteLine("3. View last 5 transactions");
            Console.WriteLine("4. Deposit money");
            Console.WriteLine("5. Change pin code");
            Console.WriteLine("6. Currency conversion");
        }

        static bool ProcessChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    ViewBalance();
                    return false;
                case "2":
                    WithdrawMoney();
                    return false;
                case "3":
                    ViewLastTransactions();
                    return false;
                case "4":
                    DepositMoney();
                    return false;
                case "5":
                    ChangePin();
                    return false;
                case "6":
                    CurrencyConversion();
                    return false;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            return true;
        }

        static void ViewBalance()
        {
            Console.WriteLine($"Your balance: ");
            Console.WriteLine($"- GEL: {userData.BalanceGEL}");
            Console.WriteLine($"- USD: {userData.BalanceUSD}");
            Console.WriteLine($"- EUR: {userData.BalanceEUR}");
        }

        static void WithdrawMoney()
        {
            Console.Write("Enter the amount to withdraw: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                if (amount <= 0)
                {
                    Console.WriteLine("Invalid amount. Please enter a positive value.");
                    return;
                }

                Console.WriteLine("Select currency:");
                Console.WriteLine("1. GEL");
                Console.WriteLine("2. USD");
                Console.WriteLine("3. EUR");
                string currencyChoice = Console.ReadLine();

                switch (currencyChoice)
                {
                    case "1":
                        if (amount > userData.BalanceGEL)
                        {
                            Console.WriteLine("Withdrawal cannot be processed.");
                            return;
                        }
                        userData.BalanceGEL -= amount;
                        break;
                    case "2":
                        if (amount > userData.BalanceUSD)
                        {
                            Console.WriteLine("Withdrawal cannot be processed.");
                            return;
                        }
                        userData.BalanceUSD -= amount;
                        break;
                    case "3":
                        if (amount > userData.BalanceEUR)
                        {
                            Console.WriteLine("Withdrawal cannot be processed.");
                            return;
                        }
                        userData.BalanceEUR -= amount;
                        break;
                    default:
                        Console.WriteLine("Invalid currency choice.");
                        return;
                }

                // ტრანზაქციის დამატება ისტორიაში
                Transaction withdrawalTransaction = new Transaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Withdrawal",
                    AmountGEL = currencyChoice == "1" ? amount : 0,
                    AmountUSD = currencyChoice == "2" ? amount : 0,
                    AmountEUR = currencyChoice == "3" ? amount : 0
                };
                userData.TransactionHistory.Add(withdrawalTransaction);

                Console.WriteLine($"Withdrawal successful. Current balance: ");
                ViewBalance();
            }
            else
            {
                Console.WriteLine("Please enter a valid numeric value.");
            }
        }

        static void DepositMoney()
        {
            Console.Write("Enter the amount to deposit: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                if (amount <= 0)
                {
                    Console.WriteLine("Invalid amount. Please enter a positive value.");
                    return;
                }

                Console.WriteLine("Select currency:");
                Console.WriteLine("1. GEL");
                Console.WriteLine("2. USD");
                Console.WriteLine("3. EUR");
                string currencyChoice = Console.ReadLine();

                switch (currencyChoice)
                {
                    case "1":
                        userData.BalanceGEL += amount;
                        break;
                    case "2":
                        userData.BalanceUSD += amount;
                        break;
                    case "3":
                        userData.BalanceEUR += amount;
                        break;
                    default:
                        Console.WriteLine("Invalid currency choice.");
                        return;
                }

                // ტრანზაქციის დამატება ისტორიაში
                Transaction depositTransaction = new Transaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Deposit",
                    AmountGEL = currencyChoice == "1" ? amount : 0,
                    AmountUSD = currencyChoice == "2" ? amount : 0,
                    AmountEUR = currencyChoice == "3" ? amount : 0
                };
                userData.TransactionHistory.Add(depositTransaction);

                Console.WriteLine($"Deposit successful. Current balance: ");
                ViewBalance();
            }
            else
            {
                Console.WriteLine("Invalid amount. Please enter a valid numeric value.");
            }
        }

        static void CurrencyConversion()
        {
            Console.WriteLine("Enter amount to convert:");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Select source currency:");
                Console.WriteLine("1. GEL");
                Console.WriteLine("2. USD");
                Console.WriteLine("3. EUR");
                string sourceCurrencyChoice = Console.ReadLine();

                Console.WriteLine("Select target currency:");
                Console.WriteLine("1. GEL");
                Console.WriteLine("2. USD");
                Console.WriteLine("3. EUR");
                string targetCurrencyChoice = Console.ReadLine();

                decimal sourceBalance;

                switch (sourceCurrencyChoice)
                {
                    case "1":
                        sourceBalance = userData.BalanceGEL;
                        break;
                    case "2":
                        sourceBalance = userData.BalanceUSD;
                        break;
                    case "3":
                        sourceBalance = userData.BalanceEUR;
                        break;
                    default:
                        Console.WriteLine("Invalid source currency choice.");
                        return;
                }

                decimal targetBalance;
                switch (targetCurrencyChoice)
                {
                    case "1":
                        targetBalance = userData.BalanceGEL;
                        break;
                    case "2":
                        targetBalance = userData.BalanceUSD;
                        break;
                    case "3":
                        targetBalance = userData.BalanceEUR;
                        break;
                    default:
                        Console.WriteLine("Invalid target currency choice.");
                        return;
                }

                Console.WriteLine("Enter conversion rate:");
                if (decimal.TryParse(Console.ReadLine(), out decimal conversionRate))
                {
                    decimal convertedAmount = amount * conversionRate;

                    if (sourceBalance >= amount)
                    {
                        sourceBalance -= amount;
                        targetBalance += convertedAmount;

                        switch (sourceCurrencyChoice)
                        {
                            case "1":
                                userData.BalanceGEL = sourceBalance;
                                break;
                            case "2":
                                userData.BalanceUSD = sourceBalance;
                                break;
                            case "3":
                                userData.BalanceEUR = sourceBalance;
                                break;
                        }

                        switch (targetCurrencyChoice)
                        {
                            case "1":
                                userData.BalanceGEL = targetBalance;
                                break;
                            case "2":
                                userData.BalanceUSD = targetBalance;
                                break;
                            case "3":
                                userData.BalanceEUR = targetBalance;
                                break;
                        }

                        // ტრანზაქციის დამატება ისტორიაში
                        Transaction conversionTransaction = new Transaction
                        {
                            TransactionDate = DateTime.Now,
                            TransactionType = "Currency Conversion",
                            AmountGEL = targetCurrencyChoice == "1" ? convertedAmount : 0,
                            AmountUSD = targetCurrencyChoice == "2" ? convertedAmount : 0,
                            AmountEUR = targetCurrencyChoice == "3" ? convertedAmount : 0
                        };
                        userData.TransactionHistory.Add(conversionTransaction);

                        Console.WriteLine($"Conversion successful. Converted amount: {convertedAmount} {targetCurrencyChoice}");
                    }
                    else
                    {
                        Console.WriteLine("Insufficient balance in source currency. Conversion cannot be processed.");
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid numeric value.");
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid numeric value.");
            }
        }

        static void ViewLastTransactions()
        {
            int count = 0;
            Console.WriteLine("Last 5 transactions:");
            for (int i = userData.TransactionHistory.Count - 1; i >= 0; i--)
            {
                if (count >= 5)
                    break;

                var transaction = userData.TransactionHistory[i];
                string currency = "";

                if (transaction.AmountGEL > 0)
                    currency = "GEL";
                else if (transaction.AmountUSD > 0)
                    currency = "USD";
                else if (transaction.AmountEUR > 0)
                    currency = "EUR";

                Console.WriteLine($"Date: {transaction.TransactionDate}, Type: {transaction.TransactionType}, Amount: {Math.Abs(transaction.AmountGEL + transaction.AmountUSD + transaction.AmountEUR)} {currency}");
                count++;
            }
        }

        static void ChangePin()
        {
            Console.Write("Enter new pin code: ");
            string newPin = Console.ReadLine();
            userData.PinCode = newPin;
            Console.WriteLine("Pin code changed successfully.");
        }

        static UserData LoadUserData(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<UserData>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user data: {ex.Message}");
                return null;
            }
        }

        static void SaveUserData(UserData userData, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(userData, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user data: {ex.Message}");
            }
        }
    }

}
