using UnityEngine;

public class Card : ScriptBase
{
    public string Nome;
    public int Ataque;
    public int Defesa;
    public int CodigoTipoCarta;

    private int ataqueAnterior;
    private int defesaAnterior;

    public Carta _Carta;

    // Use this for initialization
    void Start()
    {
        loadText();

        if (CodigoTipoCarta == 0 || CodigoTipoCarta > 5)
            CodigoTipoCarta = 1;


        _Carta = CarregarCarta(Nome, Ataque, Defesa, CodigoTipoCarta);
    }

    // Update is called once per frame
    void Update()
    {
        if (Defesa != defesaAnterior || Ataque != ataqueAnterior)
        {
            loadText();
        }
    }


    void loadText()
    {
        GetComponent<TextMesh>().text = Ataque.ToString("00") + "\t" + Defesa.ToString("00");

        ataqueAnterior = Ataque;
        defesaAnterior = Defesa;
    }
}

public partial class ScriptBase
{
    public Card Card
    {
        get { return GetComponent<Card>(); }
    }
}