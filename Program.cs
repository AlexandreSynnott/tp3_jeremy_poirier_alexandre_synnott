namespace tp3AlexJeremy
{
    internal class Program
    {
        public static int[] CalculerFrequences(string texte)
        {
            int[] frequences = new int[36]; //ca represente les 26 lettre + 10 chiffres. Donc en gros c simple: 26+10 = 36

            foreach (char c in texte)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    frequences[c - 'A']++;
                }
                else if (c >= '0' && c <= '9')
                {
                    frequences[26 + (c - '0')]++;
                }
            }

            return frequences;
        }

        public static void GenererCodes(Node<(char charactere, int freq)> node, string code, Dictionary<char, string> codes)
        {
            if (node.Left == null && node.Right == null)
            {
                codes[node.Value.charactere] = code;
                return;
            }
            if (node.Left != null)
            {
                GenererCodes(node.Left, code + "0", codes);
            }
            if (node.Right != null)
            {
                GenererCodes(node.Right, code + "1", codes);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Entrez un texte (A-Z et 0-9) : ");
            string texte = Console.ReadLine().ToUpper();

            int[] freq = CalculerFrequences(texte);

            Console.WriteLine("Fréquences des caractères : ");
            for (int i = 0; i < 26; i++)
            {
                if (freq[i] > 0)
                {
                    Console.WriteLine($"{(char)('A' + i)} : {freq[i]}");
                }
            }

            for (int i = 26; i < 36; i++)
            {
                if (freq[i] > 0)
                {
                    Console.WriteLine($"{(char)('0' + (i - 26))} : {freq[i]}");
                }
            }

            var elements = new List<(char charactere, int freq)>();
            for (int i = 0; i < 26; i++)
            {
                if (freq[i] > 0)
                {
                    elements.Add(((char)('A' + i), freq[i]));
                }
            }

            for (int i = 26; i < 36; i++)
            {
                if (freq[i] > 0)
                {
                    elements.Add(((char)('0' + (i - 26)), freq[i]));
                }
            }

            elements.Sort((a, b) => a.freq.CompareTo(b.freq));

            Node<(char charactere, int freq)>? root = null;
            foreach (var e in elements)
            {
                if (root == null)
                {
                    root = new Node<(char charactere, int freq)>(e);
                }
                else
                {
                    root.Add(e);
                }
            }

            if (root != null)
            {
                Console.WriteLine("\nArbre binaire (racine et enfants) :");
                Console.WriteLine($"Racine : {root.Value.charactere} ({root.Value.freq})");
                if (root.Left != null)
                {
                    Console.WriteLine($"Gauche : {root.Left.Value.charactere} ({root.Left.Value.freq})");
                }
                if (root.Right != null)
                {
                    Console.WriteLine($"Droite : {root.Right.Value.charactere} ({root.Right.Value.freq})");
                }
            }

            if (root != null)
            {
                var codes = new Dictionary<char, string>();
                GenererCodes(root, "", codes);
                Console.WriteLine("\nCodes binaires :");
                foreach (var charCode in codes)
                {
                    Console.WriteLine($"{charCode.Key} : {charCode.Value}");
                }
            } //revoir car ne donne pas le résultat voulu
        }
    }
}
