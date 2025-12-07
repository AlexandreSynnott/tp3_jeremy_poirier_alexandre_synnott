using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tp3AlexJeremy
{
    internal class Program
    {
        // Calcule la fréquence de chaque caractère (A-Z et 0-9) dans le texte
        // Retourne un tableau de 36 entiers : 26 pour les lettres, 10 pour les chiffres
        public static int[] CalculerFrequences(string texte)
        {
            int[] frequences = new int[36];

            foreach (char c in texte)
            {
                if (c >= 'A' && c <= 'Z') frequences[c - 'A']++;       // lettres A-Z
                else if (c >= '0' && c <= '9') frequences[26 + (c - '0')]++; // chiffres 0-9
            }

            return frequences;
        }

        // Construit l'arbre de Huffman à partir d'un dictionnaire de fréquences
        // Chaque noeud contient un caractère ou est un noeud interne avec des sous-arbres
        public static Noeud ConstruireArbre(Dictionary<char, int> freq)
        {
            var pq = new PriorityQueue<Noeud, int>(); // file de priorité selon la fréquence

            // Ajouter tous les caractères comme feuilles de l'arbre
            foreach (var kvp in freq)
                pq.Enqueue(new Noeud(kvp.Key, kvp.Value), kvp.Value);

            // Construire l'arbre en combinant les deux noeuds de plus faible fréquence
            while (pq.Count > 1)
            {
                var g = pq.Dequeue(); // noeud gauche
                var d = pq.Dequeue(); // noeud droit

                // Créer un noeud parent sans caractère avec la somme des fréquences
                var parent = new Noeud(null, g.Frequence + d.Frequence)
                {
                    Gauche = g,
                    Droite = d
                };

                pq.Enqueue(parent, parent.Frequence); // remettre le parent dans la file
            }

            return pq.Dequeue(); // retourne la racine de l'arbre
        }

        // Génère récursivement les codes binaires pour chaque caractère
        // node : noeud courant, code : code accumulé jusqu'à ce noeud
        public static void GenererCodes(Noeud node, string code, Dictionary<char, string> codes)
        {
            if (node == null) return; // arrêt si le noeud est vide

            if (node.Caractere != null) // si on atteint une feuille
            {
                if (code == "") code = "0"; // cas spécial : un seul caractère
                codes[node.Caractere.Value] = code; // assigner le code binaire
                return;
            }

            // explorer les sous-arbres gauche et droit
            GenererCodes(node.Gauche, code + "0", codes);
            GenererCodes(node.Droite, code + "1", codes);
        }

        // Décompresse une chaîne de bits en texte original en parcourant l'arbre
        public static string Decompresser(string bits, Noeud root)
        {
            StringBuilder resultat = new StringBuilder(); // construit le texte final
            Noeud courant = root; // pointeur pour parcourir l'arbre

            foreach (char bit in bits)
            {
                courant = (bit == '0') ? courant.Gauche : courant.Droite; // descendre dans l'arbre

                if (courant.Caractere != null) // si une feuille est atteinte
                {
                    resultat.Append(courant.Caractere.Value); // ajouter le caractère
                    courant = root; // revenir à la racine pour le prochain caractère
                }
            }

            return resultat.ToString(); // texte décompressé
        }

        // Point d'entrée du programme
        static void Main()
        {
            Console.WriteLine("Entrez un texte (A-Z et 0-9) : ");
            string texte = Console.ReadLine().ToUpper(); // lecture et conversion en majuscules

            // Calcul des fréquences des caractères
            int[] freqArray = CalculerFrequences(texte);

            // Conversion du tableau en dictionnaire pour simplifier l'utilisation
            var freqDict = new Dictionary<char, int>();
            for (int i = 0; i < 26; i++) if (freqArray[i] > 0) freqDict[(char)('A' + i)] = freqArray[i];
            for (int i = 26; i < 36; i++) if (freqArray[i] > 0) freqDict[(char)('0' + (i - 26))] = freqArray[i];

            // Affichage des fréquences
            Console.WriteLine("\nFréquences :");
            foreach (var kvp in freqDict) Console.WriteLine($"{kvp.Key} : {kvp.Value}");

            // Construction de l'arbre de Huffman
            Noeud racine = ConstruireArbre(freqDict);

            // Génération des codes Huffman pour chaque caractère
            var codes = new Dictionary<char, string>();
            GenererCodes(racine, "", codes);

            Console.WriteLine("\nCodes Huffman :");
            foreach (var kvp in codes) Console.WriteLine($"{kvp.Key} : {kvp.Value}");

            // Compression du texte en remplaçant chaque caractère par son code binaire
            string compresse = string.Concat(texte.Select(c => codes[c]));
            Console.WriteLine($"\nTexte compressé : {compresse}");
            Console.WriteLine($"Longueur originale : {texte.Length * 8} bits"); // taille en bits
            Console.WriteLine($"Longueur compressée : {compresse.Length} bits");   // taille après compression

            // Décompression pour vérifier que le texte original est retrouvé
            string decompresse = Decompresser(compresse, racine);
            Console.WriteLine($"\nTexte décompressé : {decompresse}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tp3AlexJeremy
{
    internal class Program
    {
        // Calcule la fréquence de chaque caractère (A-Z et 0-9) dans le texte
        // Retourne un tableau de 36 entiers : 26 pour les lettres, 10 pour les chiffres
        public static int[] CalculerFrequences(string texte)
        {
            int[] frequences = new int[36];

            foreach (char c in texte)
            {
                if (c >= 'A' && c <= 'Z') frequences[c - 'A']++;       // lettres A-Z
                else if (c >= '0' && c <= '9') frequences[26 + (c - '0')]++; // chiffres 0-9
            }

            return frequences;
        }

        // Construit l'arbre de Huffman à partir d'un dictionnaire de fréquences
        // Chaque noeud contient un caractère ou est un noeud interne avec des sous-arbres
        public static Noeud ConstruireArbre(Dictionary<char, int> freq)
        {
            var pq = new PriorityQueue<Noeud, int>(); // file de priorité selon la fréquence

            // Ajouter tous les caractères comme feuilles de l'arbre
            foreach (var kvp in freq)
                pq.Enqueue(new Noeud(kvp.Key, kvp.Value), kvp.Value);

            // Construire l'arbre en combinant les deux noeuds de plus faible fréquence
            while (pq.Count > 1)
            {
                var g = pq.Dequeue(); // noeud gauche
                var d = pq.Dequeue(); // noeud droit

                // Créer un noeud parent sans caractère avec la somme des fréquences
                var parent = new Noeud(null, g.Frequence + d.Frequence)
                {
                    Gauche = g,
                    Droite = d
                };

                pq.Enqueue(parent, parent.Frequence); // remettre le parent dans la file
            }

            return pq.Dequeue(); // retourne la racine de l'arbre
        }

        // Génère récursivement les codes binaires pour chaque caractère
        // node : noeud courant, code : code accumulé jusqu'à ce noeud
        public static void GenererCodes(Noeud node, string code, Dictionary<char, string> codes)
        {
            if (node == null) return; // arrêt si le noeud est vide

            if (node.Caractere != null) // si on atteint une feuille
            {
                if (code == "") code = "0"; // cas spécial : un seul caractère
                codes[node.Caractere.Value] = code; // assigner le code binaire
                return;
            }

            // explorer les sous-arbres gauche et droit
            GenererCodes(node.Gauche, code + "0", codes);
            GenererCodes(node.Droite, code + "1", codes);
        }

        // Décompresse une chaîne de bits en texte original en parcourant l'arbre
        public static string Decompresser(string bits, Noeud root)
        {
            StringBuilder resultat = new StringBuilder(); // construit le texte final
            Noeud courant = root; // pointeur pour parcourir l'arbre

            foreach (char bit in bits)
            {
                courant = (bit == '0') ? courant.Gauche : courant.Droite; // descendre dans l'arbre

                if (courant.Caractere != null) // si une feuille est atteinte
                {
                    resultat.Append(courant.Caractere.Value); // ajouter le caractère
                    courant = root; // revenir à la racine pour le prochain caractère
                }
            }

            return resultat.ToString(); // texte décompressé
        }

        // Point d'entrée du programme
        static void Main()
        {
            Console.WriteLine("Entrez un texte (A-Z et 0-9) : ");
            string texte = Console.ReadLine().ToUpper(); // lecture et conversion en majuscules

            // Calcul des fréquences des caractères
            int[] freqArray = CalculerFrequences(texte);

            // Conversion du tableau en dictionnaire pour simplifier l'utilisation
            var freqDict = new Dictionary<char, int>();
            for (int i = 0; i < 26; i++) if (freqArray[i] > 0) freqDict[(char)('A' + i)] = freqArray[i];
            for (int i = 26; i < 36; i++) if (freqArray[i] > 0) freqDict[(char)('0' + (i - 26))] = freqArray[i];

            // Affichage des fréquences
            Console.WriteLine("\nFréquences :");
            foreach (var kvp in freqDict) Console.WriteLine($"{kvp.Key} : {kvp.Value}");

            // Construction de l'arbre de Huffman
            Noeud racine = ConstruireArbre(freqDict);

            // Génération des codes Huffman pour chaque caractère
            var codes = new Dictionary<char, string>();
            GenererCodes(racine, "", codes);

            Console.WriteLine("\nCodes Huffman :");
            foreach (var kvp in codes) Console.WriteLine($"{kvp.Key} : {kvp.Value}");

            // Compression du texte en remplaçant chaque caractère par son code binaire
            string compresse = string.Concat(texte.Select(c => codes[c]));
            Console.WriteLine($"\nTexte compressé : {compresse}");
            Console.WriteLine($"Longueur originale : {texte.Length * 8} bits"); // taille en bits
            Console.WriteLine($"Longueur compressée : {compresse.Length} bits");   // taille après compression

            // Décompression pour vérifier que le texte original est retrouvé
            string decompresse = Decompresser(compresse, racine);
            Console.WriteLine($"\nTexte décompressé : {decompresse}");
        }
    }
}
