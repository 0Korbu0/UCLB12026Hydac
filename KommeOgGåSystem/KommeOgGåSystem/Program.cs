using KommeOgGåSystem;
using MenuSystem;

internal class Program
{
    private static void Main(string[] args)
    {
        // Laver hovedmenuen for KommeOgGåSystem
        Menu mainMenu = new Menu("HYDAC - KommeOgGåSystem");
        string[] menuNames = ["Logbog (Medarbejdere)", "Gæstebog (Besøgende)"];

        for (int i = 0; i < menuNames.Length; i++)
        {
            mainMenu.AddMenuItem(menuNames[i]);
        }

        do
        {
            mainMenu.Show();
            int menuChoice = mainMenu.SelectMenuItem();
            Console.Clear();

            // Switch case til at åbne det valgte menupunkt
            switch (menuChoice)
            {
                case 1:
                    // Åbn Medarbejder-logbogen
                    Logbog hydacLogbog = new Logbog();
                    hydacLogbog.ShowMenu();
                    break;

                case 2:
                    // Åbn Gæstebogen
                    GuestBook guestBook = new GuestBook();
                    guestBook.ShowMenu();
                    break;
            }
        } while (true);
    }
}