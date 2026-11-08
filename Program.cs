using System.Globalization;

namespace MANETrouverNombreCaché;

class Program
{
    private static double? _meilleureNote;

    static void Main(string[] args)
    {
        while (true)
        {
            Afficher("Choisissez 'F'-Facile 'D'-Difficile 'Q'-Quitter");

            char choix = LireChoixMenu();

            if (choix == 'Q')
            {
                EffacerEcran();
                return;
            }

            if (choix == 'F')
            {
                JouerFacile();
            }
            else
            {
                JouerDifficile();
            }
        }
    }

    static void JouerFacile()
    {
        const int min = 1;
        const int max = 10;

        while (true)
        {
            int nombreCache = GenererNombre(min, max);
            List<int> reponses = new List<int>();

            Afficher($"Choisissez un nombre entre [{min}, {max}] ou 'Q'-Quitter");

            while (true)
            {
                string saisie = LireSaisie();

                if (EstQuitter(saisie))
                {
                    EffacerEcran();
                    AfficherMeilleureNote();
                    return;
                }

                if (!EssayerLireEntier(saisie, out int reponse) || reponse < min || reponse > max)
                {
                    Afficher($"Réponse doit être comprise entre {min} et {max}");
                    continue;
                }

                if (reponses.Contains(reponse))
                {
                    Afficher($"Vous avez déjà proposé {reponse}, choisissez un autre nombre");
                    continue;
                }

                reponses.Add(reponse);

                if (reponse == nombreCache)
                {
                    TerminerPartieGagnee(min, max, reponses.Count);
                    break;
                }

                Afficher($"Vous avez perdu. Réponses déjà données : {string.Join(", ", reponses)}");
            }
        }
    }

    static void JouerDifficile()
    {
        while (true)
        {
            Afficher("Entrez les bornes min et max de l'intervalle (exemple : 1 20), ou 'Q'-Quitter");

            int min;
            int max;

            while (true)
            {
                string saisie = LireSaisie();

                if (EstQuitter(saisie))
                {
                    EffacerEcran();
                    AfficherMeilleureNote();
                    return;
                }

                if (EssayerLireBornes(saisie, out min, out max))
                {
                    break;
                }

                Afficher("Choisissez 2 nombres entiers et (max - min) doit être > à 10");
            }

            int nombreCache = GenererNombre(min, max);
            List<int> reponses = new List<int>();

            while (true)
            {
                Afficher($"Choisissez un nombre entre [{min}, {max}] ou 'Q'-Quitter");
                string saisie = LireSaisie();

                if (EstQuitter(saisie))
                {
                    EffacerEcran();
                    break;
                }

                if (!EssayerLireEntier(saisie, out int reponse) || reponse < min || reponse > max)
                {
                    Afficher($"Choix doit être compris entre {min} et {max}");
                    continue;
                }

                if (reponses.Contains(reponse))
                {
                    Afficher($"Vous avez déjà proposé {reponse}, choisissez un autre nombre");
                    continue;
                }

                reponses.Add(reponse);

                if (reponse == nombreCache)
                {
                    TerminerPartieGagnee(min, max, reponses.Count);
                    break;
                }

                Afficher("Vous avez perdu");
            }
        }
    }

    static void Afficher(string message)
    {
        Console.WriteLine(message);
    }

    static char LireChoixMenu()
    {
        while (true)
        {
            string reponse = LireSaisie().ToUpperInvariant();

            if (reponse == "F" || reponse == "D" || reponse == "Q")
            {
                return reponse[0];
            }

            Afficher("Choix 'F', 'D' ou 'Q'");
        }
    }

    static string LireSaisie()
    {
        string? saisie = Console.ReadLine();

        if (saisie is null)
        {
            return "Q";
        }

        return saisie.Trim();
    }

    static bool EstQuitter(string saisie)
    {
        return saisie.Equals("Q", StringComparison.OrdinalIgnoreCase);
    }

    static bool EssayerLireEntier(string texte, out int valeur)
    {
        return int.TryParse(texte, NumberStyles.Integer, CultureInfo.InvariantCulture, out valeur);
    }

    static bool EssayerLireBornes(string saisie, out int min, out int max)
    {
        min = 0;
        max = 0;

        string[] parties = saisie.Split(new[] { ' ', '\t', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (parties.Length != 2)
        {
            return false;
        }

        if (!EssayerLireEntier(parties[0], out min) || !EssayerLireEntier(parties[1], out max))
        {
            return false;
        }

        return min < max && (long)max - min > 10;
    }

    static int GenererNombre(int min, int max)
    {
        return (int)Random.Shared.NextInt64(min, (long)max + 1);
    }

    static double CalculerNote(int min, int max, int nombreEssais)
    {
        long etendue = (long)max - min + 1;
        double note = (etendue - nombreEssais) * 100.0 / etendue;
        return Math.Max(0, note);
    }

    static void TerminerPartieGagnee(int min, int max, int nombreEssais)
    {
        double note = CalculerNote(min, max, nombreEssais);

        if (_meilleureNote is null || note > _meilleureNote)
        {
            _meilleureNote = note;
        }

        Afficher("Vous avez gagné");
        Afficher($"Nombre d'essais : {nombreEssais}, note : {note:0.##} %");
        Afficher("Appuyez sur Entrée pour continuer...");
        Console.ReadLine();
        EffacerEcran();
    }

    static void AfficherMeilleureNote()
    {
        if (_meilleureNote is null)
        {
            Afficher("Aucune partie gagnée, pas encore de meilleure note.");
        }
        else
        {
            Afficher($"Meilleure note du joueur : {_meilleureNote:0.##} %");
        }
    }

    static void EffacerEcran()
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
        }
    }
}