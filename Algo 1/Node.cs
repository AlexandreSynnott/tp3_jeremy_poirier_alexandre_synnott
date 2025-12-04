using System;

namespace tp3AlexJeremy
{
    public class Noeud
    {
        public char? Caractere;   
        public int Frequence;     
        public Noeud Gauche;
        public Noeud Droite;

        public Noeud(char? c, int f)
        {
            Caractere = c;
            Frequence = f;
        }
    }
}
