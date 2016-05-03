using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading;

public partial class ScriptBase : MonoBehaviour
{
    #region Variaveis
    private int VidaInicial { get { return 20; } }
    protected int MaxVida { get { return 20; } }

    public List<TipoCarta> TiposCarta { get; private set; }

    public List<Carta> Colecao { get; private set; }
    public List<Usuario> usuarios { get; private set; }
    #endregion

    #region Carregar Objetos
    protected Carta CarregarCarta(string nome, int ataque, int defesa, int codigoTipoCarta)
    {
        const int Maximo = 12;
        const int Minimo = 2;
        const int MaximoSingle = 10;

        if (ataque > MaximoSingle)
            ataque = MaximoSingle;
        if (defesa > MaximoSingle)
            defesa = MaximoSingle;

        var x = 0;
        while ((ataque + defesa) < Minimo)
        {
            if (x % 2 > 0)
                ataque++;
            else
                defesa++;
            x++;
        }

        while ((ataque + defesa) > Maximo)
        {
            if (x % 2 > 0)
                ataque--;
            else
                defesa--;

            x++;
        }

        int codigo = 0;
        codigo++;

        var carta = new Carta()
        {
            //Codigo = codigo,
            Nome = nome,
            Ataque = ataque,
            Defesa = defesa,
            CodigoTipoCarta = codigoTipoCarta
        };

        if (Colecao == null)
            Colecao = new List<Carta>();
        else if (Colecao.Find(p => p.Nome == nome && p.CodigoTipoCarta == codigoTipoCarta) == null)
            Colecao.Add(carta);

        return carta;
    }

    protected void CarregarUsuarios()
    {
        if (usuarios == null) usuarios = new List<Usuario>();
        usuarios.Add(new Usuario()
        {
            Codigo = 1,
            Nome = "Computador",
            Vida = VidaInicial,
            Deck = new List<Carta>(),
            Campo = new List<Carta>(),
            Jogo = new List<Carta>()
        });

        usuarios.Add(new Usuario()
        {
            Codigo = 2,
            Nome = "Usuário",
            Vida = VidaInicial,
            Deck = new List<Carta>(),
            Campo = new List<Carta>(),
            Jogo = new List<Carta>()
        });
    }

    protected void CarregarTiposCarta()
    {
        if (TiposCarta == null) TiposCarta = new List<TipoCarta>();

        TiposCarta.Add(new TipoCarta()
        {
            Codigo = 1,
            Nome = "Planície"
        });

        TiposCarta.Add(new TipoCarta()
        {
            Codigo = 2,
            Nome = "Pantano"
        });

        TiposCarta.Add(new TipoCarta()
        {
            Codigo = 3,
            Nome = "Ilha"
        });

        TiposCarta.Add(new TipoCarta()
        {
            Codigo = 4,
            Nome = "Floresta"
        });
    }

    protected void CarregarCartas()
    {
        if (Colecao == null)
            Colecao = new List<Carta>();

        foreach (Transform item in GameObject.Find("Colecao").transform)
        {
            Colecao.Add(((Card)item.GetComponent("Card"))._Carta);
        }
    }
    #endregion

    #region Classes
    public class Usuario
    {
        public int Codigo { get; set; }
        public int CodigoTipoCarta { get; set; }
        public string Nome { get; set; }
        public int Vida { get; set; }
        public int Ataque
        {
            get { return Campo == null ? 0 : Campo.Sum(p => p.Ataque); }
        }
        public int Defesa
        {
            get { return Campo == null ? 0 : Campo.Sum(p => p.Defesa); }
        }

        public List<Carta> Deck { get; set; }
        public List<Carta> Campo { get; set; }
        public List<Carta> Jogo { get; set; }
    }
    public class TipoCarta
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
    }

    public enum UsuarioEN
    {
        Computador = 1,
        Usuario = 2
    }

    public enum TipoCartaEN
    {
        Planicie = 1,
        Pantano = 2,
        Ilha = 3,
        Floresta = 4,
        Montanha = 5
    }
    public class Carta
    {
        //public int Codigo { get; set; }
        public string Nome { get; set; }
        public int Ataque { get; set; }
        public int Defesa { get; set; }
        public int CodigoTipoCarta { get; set; }
    }
    #endregion

    #region Funções Complementares
    public void ThreadSleep(int time)
    {
        //Thread.Sleep(time);
        //yield return new WaitForSeconds(time);
    }
    #endregion
}
