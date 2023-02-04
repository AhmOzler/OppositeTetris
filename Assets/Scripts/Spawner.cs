using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Spawner : MonoBehaviour
{
    private static Spawner instance;
    public static Spawner Instance => instance;
    [SerializeField] [Range(0, 12)] int sqrBeginning = 5;
    [SerializeField] [Range(1, 8)] int minDensity = 7;
    [SerializeField] [Range(1, 8)] int maxDensity = 7;
    [SerializeField] [Range(.2f, 25)] float beginSpawnSpeed = 10;
    [SerializeField] [Range(10, 1)] float endSpawnSpeed = 10;
    [SerializeField] AnimationCurve difficultyCurve;
    [SerializeField] [Range(0, 10)] int bonusSqrPercentage = 1;
    [SerializeField] [Range(1, 8)] int bonusSqrPercentageMultipler = 1;
    [SerializeField] [Range(0, 10)] int fillEmptyBoard;
    [SerializeField] Transform pointSquare;
    [SerializeField] Transform bonusSquare;
    Transform topShapeHolder;
    private float timer;
    private float cooldown;
    private float spawnTime;



    private void Awake() 
    {
        if(!instance) 
        {
            instance = this;
        }
        else {
            instance = null;
            Destroy(gameObject);
        }
    }



    private void Start() 
    {           
        topShapeHolder = new GameObject("SquareHolder").transform;
        Board.Instance.OnSquareDestroy += SpawnWhenEmpty;
    }
    


    private Transform Sqr() //ANCHOR - Belirtilen yüzde oranında bonusSquare yada topSquare döndürür.
    {
        float count = (float) Board.Instance.TotalDestroyedSquaresCount;

        if(UnityEngine.Random.Range(0, 100) < bonusSqrPercentage * Mathf.Lerp(1f, bonusSqrPercentageMultipler, difficultyCurve.Evaluate(count / 1000f)))
        {
            return bonusSquare;
        }

        return pointSquare;
    }



    public List<int> DigitList(int minDensity, int maxDensity) //ANCHOR - Srq ların sıklığını ve pozisyonlarını belirler.
    {
        List<int> sqrDigits = new List<int>();
        sqrDigits.Clear();
        
        int density = (int) UnityEngine.Random.Range (minDensity, maxDensity + 1);

        while (sqrDigits.Distinct().ToList().Count < density) //ANCHOR - Liste'ye aynı eleman eklendiğinde listenin eleman sayısı çoğalmasın diye distinct kullanılarak liste'nin eleman sayısı kontrol edildi.
        {
            int randomPosX = (int) UnityEngine.Random.Range (0, Board.Instance.BoardWidth);
            sqrDigits.Add(randomPosX);
        }

        return sqrDigits.Distinct().ToList();
    }



    public void CreateSqr (Board board, List<int> sqrDigits) //ANCHOR - Sqr üretir.
    {
        for (int r = 0; r < sqrDigits.Count; r++)
        {
            Vector2 randomXpos = new Vector2 (sqrDigits[r], Board.Instance.BoardHeight - 1); //ANCHOR Sqrların x ekseninde ki random pozisyonları.
        
            Transform sqr = Instantiate(Sqr(), randomXpos, Quaternion.identity, topShapeHolder);

            int hologramFadePropertyID = Shader.PropertyToID("_HologramFade"); //ANCHOR Hologram'ın fade ini 0 lamak için.
            var mat = sqr.GetComponent<SpriteRenderer>().material;
            mat.SetFloat(hologramFadePropertyID, 0);

            board.StoreShapeInGrid(sqr);
        }
    }



    public IEnumerator SpawnSqrAtStart() 
    {
        Board board = Board.Instance;
        List<int> sqrDigits;

        yield return new WaitUntil(() => UIController.Instance.IsUIWallOpen);

        for (int i = 0; i < sqrBeginning; i++) //ANCHOR Oyun başlangıcında TopSqr üretmek için.
        {
            sqrDigits = DigitList(minDensity, maxDensity);
            board.ShiftRowDown();          
            CreateSqr(board, sqrDigits);           
        }

        SoundManager.Instance.Play("TeleportBricks");
    }



    public void SpawnInTime()
    {
        timer += Time.deltaTime;

        if (timer >= cooldown)
        {    
            float count = (float) Board.Instance.TotalDestroyedSquaresCount;
            
            spawnTime = Mathf.Lerp(beginSpawnSpeed, endSpawnSpeed, difficultyCurve.Evaluate(count / 1000f));           
            cooldown += spawnTime;

            StartCoroutine("TimeBarHighlight", .3f);

            Board.Instance.ShiftRowDown();
            SoundManager.Instance.Play("TeleportBricks");
            CreateSqr(Board.Instance, DigitList(minDensity, maxDensity));
        }

        UIController.Instance.SetTimeBarShader("_SourceGlowDissolveFade", ((cooldown - timer) * 9 / beginSpawnSpeed));

        //Debug.Log("cooldown : " + cooldown);
    }



    private void SpawnWhenEmpty() 
    {       
        for (int i = 0; i < fillEmptyBoard; i++)
        {        
            Board.Instance.ShiftRowDown();
            CreateSqr(Board.Instance, DigitList(minDensity, maxDensity));
        } 

        timer = 0;
        cooldown = spawnTime;

        SoundManager.Instance.Play("TeleportBricks");
    }



    public void BonusTimeButton(int value) //ANCHOR - ChangeButton'un üstünde event olarak kullanılıyor.
    {
        if(UIController.Instance.ChangeButtonCount < value) return;

        if((cooldown - timer) <= beginSpawnSpeed)
            cooldown += value * 2;


        if((cooldown - timer) > beginSpawnSpeed) 
            cooldown = timer + beginSpawnSpeed;
        
        float fade = value <= 1 ? fade = .15f : .3f;

        IEnumerator num = UIController.Instance.TimeBarHighlight(fade);

        StartCoroutine(num);               
    }



    IEnumerator TimeBarHighlight(float fadeValue) { //ANCHOR - BonusTimeButton methodunda kullanılıyor.

        float fade = fadeValue;
        
        while(fade >= 0 ) 
        {
            fade -= Time.deltaTime;
            UIController.Instance.SetTimeBarShader("_RecolorFade", fade);
            yield return null;
        }
    }
}
