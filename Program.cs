using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tp3AlexJeremy
{
    internal class Program
    {
        // Calcul des fréquences
        public static int[] CalculerFrequences(string texte)
        {
            int[] frequences = new int[36]; // 26 lettres + 10 chiffres

            foreach (char c in texte)
            {
                if (c >= 'A' && c <= 'Z') frequences[c - 'A']++;
                else if (c >= '0' && c <= '9') frequences[26 + (c - '0')]++;
            }

            return frequences;
        }

        // Construction de l'arbre de Huffman
        public static Noeud ConstruireArbre(Dictionary<char, int> freq)
        {
            var pq = new PriorityQueue<Noeud, int>();

            foreach (var kvp in freq)
                pq.Enqueue(new Noeud(kvp.Key, kvp.Value), kvp.Value);

            while (pq.Count > 1)
            {
                var g = pq.Dequeue();
                var d = pq.Dequeue();
                var parent = new Noeud(null, g.Frequence + d.Frequence)
                {
                    Gauche = g,
                    Droite = d
                };
                pq.Enqueue(parent, parent.Frequence);
            }

            return pq.Dequeue();
        }

        // Génération des codes binaires
        public static void GenererCodes(Noeud node, string code, Dictionary<char, string> codes)
        {
            if (node == null) return;

            if (node.Caractere != null)
            {
                if (code == "") code = "0"; // éviter code vide si un seul caractère
                codes[node.Caractere.Value] = code;
                return;
            }

            GenererCodes(node.Gauche, code + "0", codes);
            GenererCodes(node.Droite, code + "1", codes);
        }

        // Décompression
        public static string Decompresser(string bits, Noeud root)
        {
            StringBuilder resultat = new StringBuilder();
            Noeud courant = root;

            foreach (char bit in bits)
            {
                courant = (bit == '0') ? courant.Gauche : courant.Droite;

                if (courant.Caractere != null)
                {
                    resultat.Append(courant.Caractere.Value);
                    courant = root;
                }
            }

            return resultat.ToString();
        }

        static void Main()
        {
            Console.WriteLine("Entrez un texte (A-Z et 0-9) : ");
            string texte = Console.ReadLine().ToUpper();

            int[] freqArray = CalculerFrequences(texte);

            // Conversion en dictionnaire
            var freqDict = new Dictionary<char, int>();
            for (int i = 0; i < 26; i++) if (freqArray[i] > 0) freqDict[(char)('A' + i)] = freqArray[i];
            for (int i = 26; i < 36; i++) if (freqArray[i] > 0) freqDict[(char)('0' + (i - 26))] = freqArray[i];

            Console.WriteLine("\nFréquences :");
            foreach (var kvp in freqDict) Console.WriteLine($"{kvp.Key} : {kvp.Value}");

            // Arbre Huffman
            Noeud racine = ConstruireArbre(freqDict);

            // Codes binaires
            var codes = new Dictionary<char, string>();
            GenererCodes(racine, "", codes);

            Console.WriteLine("\nCodes Huffman :");
            foreach (var kvp in codes) Console.WriteLine($"{kvp.Key} : {kvp.Value}");

            // Compression
            string compresse = string.Concat(texte.Select(c => codes[c]));
            Console.WriteLine($"\nTexte compressé : {compresse}");
            Console.WriteLine($"Longueur originale : {texte.Length * 8} bits");
            Console.WriteLine($"Longueur compressée : {compresse.Length} bits");

            // Décompression
            string decompresse = Decompresser(compresse, racine);
            Console.WriteLine($"\nTexte décompressé : {decompresse}");
        }
    }
}
