using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartGame : ScriptBase
{
    protected const float DistanciaX = 4.82F;
    protected const float DistanciaXMaior = 4.6F;
    protected const string Texto1 = "ATAQUE: {0}\nDEFESA: {1}";
    protected const string Texto2 = "{0}    VIDA: {1}";
    protected const string Texto3 = "Computador: {0}\n\n       Usuário: {1}";
    protected int VitoriaMaquina = 0, VitoriaUsuario = 0;

    void Start()
    {
        CarregarUsuarios();
        CarregarTiposCarta();
        CarregarCartas();

        usuarios[(int)UsuarioEN.Computador - 1].Deck.AddRange(Colecao.Where(p => p.CodigoTipoCarta == (int)TipoCartaEN.Floresta));
        usuarios[(int)UsuarioEN.Computador - 1].Deck.AddRange(usuarios[(int)UsuarioEN.Computador - 1].Deck);//2cartas de cada tipo

        usuarios[(int)UsuarioEN.Usuario - 1].Deck.AddRange(Colecao.Where(p => p.CodigoTipoCarta == (int)TipoCartaEN.Planicie));
        usuarios[(int)UsuarioEN.Usuario - 1].Deck.AddRange(usuarios[(int)UsuarioEN.Usuario - 1].Deck);//2cartas de cada tipo

        StartCoroutine("ComprarCarta", UsuarioEN.Computador);
        StartCoroutine("ComprarCarta", UsuarioEN.Computador);
        StartCoroutine("ComprarCarta", UsuarioEN.Computador);
        StartCoroutine("ComprarCarta", UsuarioEN.Computador);
        StartCoroutine("ComprarCarta", UsuarioEN.Computador);
        StartCoroutine("ComprarCarta", UsuarioEN.Computador);
        StartCoroutine("ComprarCarta", UsuarioEN.Computador);

        StartCoroutine("ComprarCarta", UsuarioEN.Usuario);
        StartCoroutine("ComprarCarta", UsuarioEN.Usuario);
        StartCoroutine("ComprarCarta", UsuarioEN.Usuario);
        StartCoroutine("ComprarCarta", UsuarioEN.Usuario);
        StartCoroutine("ComprarCarta", UsuarioEN.Usuario);
        StartCoroutine("ComprarCarta", UsuarioEN.Usuario);
        StartCoroutine("ComprarCarta", UsuarioEN.Usuario);
    }
    void Update()
    {
        if (Input.GetKeyDown("0")) KeyDown(0);
        else if (Input.GetKeyDown("1")) KeyDown(1);
        else if (Input.GetKeyDown("2")) KeyDown(2);
        else if (Input.GetKeyDown("3")) KeyDown(3);
        else if (Input.GetKeyDown("4")) KeyDown(4);
        else if (Input.GetKeyDown("5")) KeyDown(5);
        else if (Input.GetKeyDown("6")) KeyDown(6);
        else if (Input.GetKeyDown("7")) KeyDown(7);
        else if (Input.GetKeyDown("8")) KeyDown(8);
        else if (Input.GetKeyDown("9")) KeyDown(9);
    }

    public void KeyDown(int key)
    {
        if (usuarios != null)
        {
            float PosX = key > 0 ? (key - 1) * DistanciaX : (key + 9) * DistanciaX;//ParentPosition

            var jogo = GameObject.Find("Jogo").transform;
            Transform tCarta = null;

            if (usuarios[(int)UsuarioEN.Usuario - 1].Campo.Count < 7)
            {

                foreach (Transform item in jogo)
                {
                    if (item.localPosition.x.ToString("N2") == PosX.ToString("N2"))
                    {
                        tCarta = item;
                    }
                    else if (item.localPosition.x > PosX)
                    {
                        item.localPosition = new Vector3(item.localPosition.x - DistanciaX, 0);
                    }
                }
            }
            else
            {
                usuarios[(int)UsuarioEN.Computador - 1].Vida = 0;
                usuarios[(int)UsuarioEN.Usuario - 1].Vida = 0;

                RecarregarElementos();
            }

            if (tCarta != null)
            {
                JogarCarta(UsuarioEN.Usuario, tCarta, null);

                StartCoroutine("ComprarCarta", UsuarioEN.Computador);
                JogadaMaquina();

                //TODO: Rever Ordem
                //ThreadSleep(500);
                Atacar(UsuarioEN.Usuario);

                //ThreadSleep(500);
                Atacar(UsuarioEN.Computador);

                StartCoroutine("ComprarCarta", UsuarioEN.Usuario);
            }
        }
    }

    private void JogarCarta(UsuarioEN codigoUsuario, Transform tCarta, Carta objCarta)
    {
        var usuario = usuarios[(int)codigoUsuario - 1];

        if (usuario.Campo.Count < 7)
        {
            if (tCarta != null)
            {
                objCarta = ((Card)tCarta.GetComponent("Card"))._Carta;
            }
            else
            {
                string tipoCarta = string.Empty;
                switch ((TipoCartaEN)objCarta.CodigoTipoCarta)
                {
                    case TipoCartaEN.Floresta: tipoCarta = "Forest"; break;
                    case TipoCartaEN.Ilha: tipoCarta = "Island"; break;
                    case TipoCartaEN.Montanha: tipoCarta = "Mountain"; break;
                    case TipoCartaEN.Pantano: tipoCarta = "Swamp"; break;
                    case TipoCartaEN.Planicie: tipoCarta = "Plain"; break;
                }
                tCarta = (Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/" + tipoCarta + "/" + objCarta.Nome + ".prefab", typeof(GameObject)))).transform;
            }

            int i = 0;
            bool find = true;
            while (find)
            {
                if (usuario.Jogo[i].CodigoTipoCarta == objCarta.CodigoTipoCarta &&
                    usuario.Jogo[i].Nome == objCarta.Nome)
                {
                    usuario.Jogo.RemoveAt(i);
                    find = false;
                }
                i++;
            }

            usuario.Campo.Add(objCarta);

            Transform parent = null;
            if (codigoUsuario == UsuarioEN.Usuario)
            {
                parent = GameObject.Find("Usuario").transform;
            }
            else
            {
                parent = GameObject.Find("Inimigo").transform;
            }

            if (parent != null)
            {
                float posX = ((usuario.Campo.Count - 1) * DistanciaXMaior);
                tCarta.parent = parent;
                tCarta.transform.localScale = new Vector3(1, 1);
                tCarta.transform.localPosition = new Vector3(posX, 0);
            }
        }
        else
        {
            Debug.Log("Número máximo de cartas em campo é de 7");
        }
    }
    private void RecarregarElementos()
    {
        var usuario = usuarios[(int)UsuarioEN.Usuario - 1];
        var maquina = usuarios[(int)UsuarioEN.Computador - 1];

        if (usuario.Vida <= 0 || maquina.Vida <= 0)
        {
            usuario.Campo = new List<Carta>();
            maquina.Campo = new List<Carta>();

            foreach (Transform transform in GameObject.Find("Usuario").transform)
            {
                GameObject.Destroy(transform.gameObject);
            }

            foreach (Transform transform in GameObject.Find("Inimigo").transform)
            {
                GameObject.Destroy(transform.gameObject);
            }

            if (usuario.Vida <= 0 && maquina.Vida <= 0)
            {
                VitoriaMaquina += 1;
                VitoriaUsuario += 1;
            }
            else if (maquina.Vida <= 0)
            {
                VitoriaUsuario += 1;
            }
            else if (usuario.Vida <= 0)
            {
                VitoriaMaquina += 1;
            }
            usuario.Vida = MaxVida;
            maquina.Vida = MaxVida;
        }

        GameObject.Find("Usuario 1").GetComponent<TextMesh>().text = string.Format(Texto1, usuario.Ataque.ToString("000"), usuario.Defesa.ToString("000"));
        GameObject.Find("Usuario 2").GetComponent<TextMesh>().text = string.Format(Texto2, usuario.Deck.Count.ToString("00"), usuario.Vida.ToString("00"));
        GameObject.Find("Maquina 1").GetComponent<TextMesh>().text = string.Format(Texto1, maquina.Ataque.ToString("000"), maquina.Defesa.ToString("000"));
        GameObject.Find("Maquina 2").GetComponent<TextMesh>().text = string.Format(Texto2, maquina.Deck.Count.ToString("00"), maquina.Vida.ToString("00"));
        GameObject.Find("Vitorias").GetComponent<TextMesh>().text = string.Format(Texto3, VitoriaMaquina.ToString("00"), VitoriaUsuario.ToString("00"));
    }
    private void Atacar(UsuarioEN codigoUsuarioAtacante)
    {
        var usuarioAtacante = usuarios.Find(p => p.Codigo == (int)codigoUsuarioAtacante);
        var usuarioAtacado = usuarios.Find(p => p.Codigo != (int)codigoUsuarioAtacante);

        int ataque = usuarioAtacante.Ataque - usuarioAtacado.Defesa;
        if (ataque < 0) { ataque = 0; }

        usuarioAtacado.Vida -= ataque;
    }
    private void JogadaMaquina()
    {
        var usuario = usuarios.Find(p => p.Codigo == (int)UsuarioEN.Usuario);
        var computador = usuarios.Find(p => p.Codigo == (int)UsuarioEN.Computador);


        Carta melhorEscolha = null;
        if (computador.Ataque < (usuario.Defesa - 10))
        {
            //Maior Ataque
            melhorEscolha = computador.Jogo.OrderByDescending(p => p.Ataque).FirstOrDefault();
        }
        else if (computador.Defesa < (usuario.Ataque - 10))
        {
            //Maior Defesa
            melhorEscolha = computador.Jogo.OrderByDescending(p => p.Defesa).FirstOrDefault();
        }
        else if (computador.Ataque < usuario.Defesa)
        {
            /* Ataque > Defesa && Ataque - Defesa <= 2 */
            foreach (var item in computador.Jogo)
            {
                if (item.Ataque > item.Defesa && item.Ataque - item.Defesa <= 2)
                {
                    if (melhorEscolha == null)
                    {
                        melhorEscolha = item;
                    }
                    else if (item.Ataque > melhorEscolha.Ataque)
                    {
                        melhorEscolha = item;
                    }
                }
            }
            if (melhorEscolha == null)
            {
                melhorEscolha = computador.Jogo.FirstOrDefault();
            }
        }
        else if (computador.Defesa < usuario.Ataque)
        {
            /* Ataque < Defesa && Defesa - Ataque <= 2 */
            foreach (var item in computador.Jogo)
            {
                if (item.Defesa > item.Ataque && item.Defesa - item.Ataque <= 2)
                {
                    if (melhorEscolha == null)
                    {
                        melhorEscolha = item;
                    }
                    else if (item.Defesa > melhorEscolha.Defesa)
                    {
                        melhorEscolha = item;
                    }
                }
            }
            if (melhorEscolha == null)
            {
                melhorEscolha = computador.Jogo.FirstOrDefault();
            }
        }
        else
        {
            /* Maior Somatória */
            melhorEscolha = computador.Jogo.OrderByDescending(p => p.Ataque + p.Defesa).FirstOrDefault();
        }

        JogarCarta(UsuarioEN.Computador, null, melhorEscolha);
    }

    private void ComprarCarta(UsuarioEN codigoUsuario)
    {
        //ThreadSleep(100);

        var usuario = usuarios[(int)codigoUsuario - 1];

        if (usuario.Jogo.Count < 7 && usuario.Deck.Count > 0)
        {
            System.Random r = new System.Random();
            int carta = r.Next(usuario.Deck.Count - 1);
            var objCarta = usuario.Deck[carta];

            if ((UsuarioEN)codigoUsuario == UsuarioEN.Usuario)
            {
                string tipoCarta = string.Empty;

                switch ((TipoCartaEN)objCarta.CodigoTipoCarta)
                {
                    case TipoCartaEN.Floresta: tipoCarta = "Forest"; break;
                    case TipoCartaEN.Ilha: tipoCarta = "Island"; break;
                    case TipoCartaEN.Montanha: tipoCarta = "Mountain"; break;
                    case TipoCartaEN.Pantano: tipoCarta = "Swamp"; break;
                    case TipoCartaEN.Planicie: tipoCarta = "Plain"; break;
                }

                Transform tCarta = null;

                try
                {
                    tCarta = (Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/" + tipoCarta + "/" + objCarta.Nome + ".prefab", typeof(GameObject)))).transform;
                }
                catch (System.Exception) { Debug.Log("Assets/Prefab/" + tipoCarta + "/" + objCarta.Nome + ".prefab"); }

                if (tCarta != null)
                {
                    float posX = (usuario.Jogo.Count * DistanciaX);
                    tCarta.parent = GameObject.Find("Jogo").transform;
                    tCarta.transform.localScale = new Vector3(1, 1);
                    tCarta.transform.localPosition = new Vector3(posX, 0);
                }
            }

            usuarios[(int)codigoUsuario - 1].Jogo.Add(objCarta);
            usuarios[(int)codigoUsuario - 1].Deck.RemoveAt(carta);

            RecarregarElementos();
        }
        else
        {
            Debug.LogError("Só pode ter até 7 cartas");
        }
    }
}

public partial class ScriptBase
{
    public StartGame StartGame
    {
        get { return GetComponent<StartGame>(); }
    }
}