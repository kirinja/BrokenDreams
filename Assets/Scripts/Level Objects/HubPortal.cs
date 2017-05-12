using UnityEngine;


public class HubPortal : MonoBehaviour
{
    public Color DonePortalColor, DoneEmbersColor, DoneEmbers2Color, DoneGlowColor, DoneMiddleColor;
    public Color NotDonePortalColor, NotDoneEmbersColor, NotDoneEmbers2Color, NotDoneGlowColor, NotDoneMiddleColor;
    public bool Done = false;
    public bool ShouldAppear = false;
    public bool ShouldInitialize = true;
    public float LerpTime = 2f;
    public string Scene;

    private Timer _colorLerpTimer;
    private bool _changing;
    private bool _done;
    private ParticleSystem.Particle[] _glowParticles;
    private int _glowTotalParticles;


    // Use this for initialization
    private void Start()
    {
        if (ShouldInitialize)
        {
            Done = GameManager.Instance.LevelCleared(Scene);
            gameObject.SetActive(GameManager.Instance.LevelAvailable(Scene));
        }

        _done = Done;
        SetColors(Done);

        _colorLerpTimer = new Timer(LerpTime);
        _changing = false;
        _glowTotalParticles = transform.Find("Glow").GetComponent<ParticleSystem>().particleCount;
        _glowParticles = new ParticleSystem.Particle[_glowTotalParticles];
    }


    // Update is called once per frame
    private void Update()
    {
        if (ShouldAppear && !_changing)
        {
            SetColors(false);
            _changing = true;
            _colorLerpTimer.Reset();
        }

        else if (_done != Done)
        {
            _done = Done;
            _changing = true;
            _colorLerpTimer.Reset();
        }

        if (_changing)
        {
            if (_colorLerpTimer.Update(Time.deltaTime))
            {
                _changing = false;
                ShouldAppear = false;
            }
            else
            {
                if (ShouldAppear)
                    Appear();
                else
                    LerpColors(_done);
            }
        }
    }


    private void LerpColors(bool done)
    {
        if (done)
        {
            var portalMain = GetComponent<ParticleSystem>().main;
            portalMain.startColor = Color.Lerp(NotDonePortalColor,
                DonePortalColor, _colorLerpTimer.PercentDone);

            var embersMain = transform.Find("Embers").GetComponent<ParticleSystem>().main;
            embersMain.startColor = Color.Lerp(NotDoneEmbersColor, DoneEmbersColor, _colorLerpTimer.PercentDone);

            var embers2Main = transform.Find("Embers2").GetComponent<ParticleSystem>().main;
            embers2Main.startColor = Color.Lerp(NotDoneEmbers2Color, DoneEmbers2Color, _colorLerpTimer.PercentDone);

            _glowTotalParticles = transform.Find("Glow").GetComponent<ParticleSystem>().GetParticles(_glowParticles);
            for (var i = 0; i < _glowTotalParticles; ++i)
            {
                _glowParticles[i].startColor = Color.Lerp(
                    NotDoneGlowColor, DoneGlowColor, _colorLerpTimer.PercentDone);
            }
            transform.Find("Glow").GetComponent<ParticleSystem>().SetParticles(_glowParticles, _glowTotalParticles);
            var glowMain = transform.Find("Glow").GetComponent<ParticleSystem>().main;
            glowMain.startColor = Color.Lerp(NotDoneGlowColor, DoneGlowColor, _colorLerpTimer.PercentDone);

            var middleMain = transform.Find("Middle").GetComponent<ParticleSystem>().main;
            middleMain.startColor = Color.Lerp(NotDoneMiddleColor, DoneMiddleColor, _colorLerpTimer.PercentDone);
        }
        else
        {
            var portalMain = GetComponent<ParticleSystem>().main;
            portalMain.startColor = Color.Lerp(DonePortalColor, NotDonePortalColor, _colorLerpTimer.PercentDone);

            var embersMain = transform.Find("Embers").GetComponent<ParticleSystem>().main;
            embersMain.startColor = Color.Lerp(DoneEmbersColor, NotDoneEmbersColor, _colorLerpTimer.PercentDone);

            var embers2Main = transform.Find("Embers2").GetComponent<ParticleSystem>().main;
            embers2Main.startColor = Color.Lerp(DoneEmbers2Color, NotDoneEmbers2Color, _colorLerpTimer.PercentDone);

            _glowTotalParticles = transform.Find("Glow").GetComponent<ParticleSystem>().GetParticles(_glowParticles);
            for (var i = 0; i < _glowTotalParticles; ++i)
            {
                _glowParticles[i].startColor = Color.Lerp(
                    DoneGlowColor, NotDoneGlowColor, _colorLerpTimer.PercentDone);
            }
            transform.Find("Glow").GetComponent<ParticleSystem>().SetParticles(_glowParticles, _glowTotalParticles);
            var glowMain = transform.Find("Glow").GetComponent<ParticleSystem>().main;
            glowMain.startColor = Color.Lerp(DoneGlowColor, NotDoneGlowColor, _colorLerpTimer.PercentDone);

            var middleMain = transform.Find("Middle").GetComponent<ParticleSystem>().main;
            middleMain.startColor = Color.Lerp(DoneMiddleColor, NotDoneMiddleColor, _colorLerpTimer.PercentDone);
        }
    }


    public void Hide()
    {
        gameObject.SetActive(false);

        GetComponent<ParticleSystem>().Clear();
        transform.Find("Embers").GetComponent<ParticleSystem>().Clear();
        transform.Find("Embers2").GetComponent<ParticleSystem>().Clear();
        transform.Find("Glow").GetComponent<ParticleSystem>().Clear();
        transform.Find("Middle").GetComponent<ParticleSystem>().Clear();

        var portalMain = GetComponent<ParticleSystem>().main;
        portalMain.startColor = new Color((Done ? DonePortalColor : NotDonePortalColor).r,
            (Done ? DonePortalColor : NotDonePortalColor).g,
            (Done ? DonePortalColor : NotDonePortalColor).b, 0f);

        var embersMain = transform.Find("Embers").GetComponent<ParticleSystem>().main;
        embersMain.startColor = new Color((Done ? DonePortalColor : NotDonePortalColor).r,
            (Done ? DonePortalColor : NotDonePortalColor).g,
            (Done ? DonePortalColor : NotDonePortalColor).b, 0f);

        var embers2Main = transform.Find("Embers2").GetComponent<ParticleSystem>().main;
        embers2Main.startColor = new Color((Done ? DoneEmbers2Color : NotDoneEmbers2Color).r,
            (Done ? DoneEmbers2Color : NotDoneEmbers2Color).g,
            (Done ? DoneEmbers2Color : NotDoneEmbers2Color).b, 0f);

        var glowMain = transform.Find("Glow").GetComponent<ParticleSystem>().main;
        glowMain.startColor = new Color((Done ? DoneGlowColor : NotDoneGlowColor).r,
            (Done ? DoneGlowColor : NotDoneGlowColor).g,
            (Done ? DoneGlowColor : NotDoneGlowColor).b, 0f);

        var middleMain = transform.Find("Middle").GetComponent<ParticleSystem>().main;
        middleMain.startColor = new Color((Done ? DoneMiddleColor : NotDoneMiddleColor).r,
            (Done ? DoneMiddleColor : NotDoneMiddleColor).g,
            (Done ? DoneMiddleColor : NotDoneMiddleColor).b, 0f);
    }


    private void Appear()
    {
        var portalMain = GetComponent<ParticleSystem>().main;
        portalMain.startColor = Color.Lerp(new Color((Done ? DonePortalColor : NotDonePortalColor).r,
                (Done ? DonePortalColor : NotDonePortalColor).g,
                (Done ? DonePortalColor : NotDonePortalColor).b, 0f), Done ? DonePortalColor : NotDonePortalColor,
           _colorLerpTimer.PercentDone);

        var embersMain = transform.Find("Embers").GetComponent<ParticleSystem>().main;
        embersMain.startColor = Color.Lerp(new Color((Done ? DonePortalColor : NotDonePortalColor).r,
            (Done ? DonePortalColor : NotDonePortalColor).g,
            (Done ? DonePortalColor : NotDonePortalColor).b, 0f), Done ? DoneEmbersColor : NotDoneEmbersColor, _colorLerpTimer.PercentDone);

        var embers2Main = transform.Find("Embers2").GetComponent<ParticleSystem>().main;
        embers2Main.startColor = Color.Lerp(new Color((Done ? DoneEmbers2Color : NotDoneEmbers2Color).r,
                (Done ? DoneEmbers2Color : NotDoneEmbers2Color).g,
                (Done ? DoneEmbers2Color : NotDoneEmbers2Color).b, 0f), Done ? DoneEmbers2Color : NotDoneEmbers2Color,
            _colorLerpTimer.PercentDone);

        _glowTotalParticles = transform.Find("Glow").GetComponent<ParticleSystem>().GetParticles(_glowParticles);
        for (var i = 0; i < _glowTotalParticles; ++i)
        {
            _glowParticles[i].startColor = Color.Lerp(new Color((Done ? DoneGlowColor : NotDoneGlowColor).r,
                    (Done ? DoneGlowColor : NotDoneGlowColor).g,
                    (Done ? DoneGlowColor : NotDoneGlowColor).b, 0f), Done ? DoneGlowColor : NotDoneGlowColor,
                _colorLerpTimer.PercentDone);
        }

        transform.Find("Glow").GetComponent<ParticleSystem>().SetParticles(_glowParticles, _glowTotalParticles);
        var glowMain = transform.Find("Glow").GetComponent<ParticleSystem>().main;
        glowMain.startColor = Color.Lerp(new Color((Done ? DoneGlowColor : NotDoneGlowColor).r,
                (Done ? DoneGlowColor : NotDoneGlowColor).g,
                (Done ? DoneGlowColor : NotDoneGlowColor).b, 0f), Done ? DoneGlowColor : NotDoneGlowColor,
            _colorLerpTimer.PercentDone);

        var middleMain = transform.Find("Middle").GetComponent<ParticleSystem>().main;
        middleMain.startColor = Color.Lerp(new Color((Done ? DoneMiddleColor : NotDoneMiddleColor).r,
                (Done ? DoneMiddleColor : NotDoneMiddleColor).g,
                (Done ? DoneMiddleColor : NotDoneMiddleColor).b, 0f), Done ? DoneMiddleColor : NotDoneMiddleColor,
            _colorLerpTimer.PercentDone);
    }
    

    private void SetColors(bool done)
    {
        var portalMain = GetComponent<ParticleSystem>().main;
        portalMain.startColor = done ? DonePortalColor : NotDonePortalColor;

        var embersMain = transform.Find("Embers").GetComponent<ParticleSystem>().main;
        embersMain.startColor = done ? DoneEmbersColor : NotDoneEmbersColor;

        var embers2Main = transform.Find("Embers2").GetComponent<ParticleSystem>().main;
        embers2Main.startColor = done ? DoneEmbers2Color : NotDoneEmbers2Color;

        var glowMain = transform.Find("Glow").GetComponent<ParticleSystem>().main;
        glowMain.startColor = done ? DoneGlowColor : NotDoneGlowColor;

        var middleMain = transform.Find("Middle").GetComponent<ParticleSystem>().main;
        middleMain.startColor = done ? DoneMiddleColor : NotDoneMiddleColor;
    }
}