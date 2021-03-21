using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class YuzukHareket : MonoBehaviour
{
    Rigidbody rb;
    GameObject modellerChild;


    GameObject playerDiamond;
    GameObject tac;



    float forwardSpeed = 350; // serilazefield ve range attributunu kapattım debug modda artık.

    [Range(10, 500), SerializeField]
    float tekerlekDonusHizi;

    [Range(0, 10000), SerializeField]
    float swipeSpeed;



    [Range(0.01f, 5), SerializeField]
    float _diamondSizeChangeAmount = 0.1f;

    //[Range(0.01f, 5), SerializeField]
    //float elmasKuculmeMiktari = 0.1f;

    Touch touch;
    Vector3 firstTouchPos;
    float deltaSwipeX;
    float swipeAmount=1;


    Vector3 forceVector;
    AudioSource audioSource;

    ParticleSystem dumanPrefab;
    bool isDriftSoundPlayed;

    
    Sequence tacSequence;
    [SerializeField]
    private Ease tacEase;

    MeshRenderer playerDiamondMeshRenderer;
    ParticleSystem toplamaEfekti;
    ParticleSystem renkDegismeEfekti;
    Color endColorValue;
    Material diamondMat;
    // Normal renk

    [SerializeField]
    Color redColor;
    [SerializeField]
    Color blueColor;
    [SerializeField]
    Color greenColor;

    // emission renk

    Sequence surekliHareketMove;

    [SerializeField]
    GameObject finishParticlesGameObject;



    bool stop = false;
    bool _isStartGame;
    public bool IsStartGame { get { return _isStartGame; } set {_isStartGame=value; } }
    public float ForwardSpeed { get { return forwardSpeed; } set { forwardSpeed = value; } }
    public float DiamondSizeChangeAmount { get { return _diamondSizeChangeAmount; } set { _diamondSizeChangeAmount = value; } }
    public float SwipeAmount { get { return swipeAmount; } set {swipeAmount=value;} }


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        modellerChild = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

        tac = gameObject.transform.GetChild(2).gameObject;
        playerDiamond = tac.transform.GetChild(0).gameObject;

        playerDiamondMeshRenderer = playerDiamond.gameObject.GetComponent<MeshRenderer>();
        toplamaEfekti = gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
        renkDegismeEfekti = gameObject.transform.GetChild(2).gameObject.transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();

        audioSource = gameObject.GetComponent<AudioSource>();

        diamondMat = playerDiamondMeshRenderer.material;
        diamondMat.color = blueColor;
        endColorValue = diamondMat.color;

        StartCoroutine(YuzukTaciAnimCoroutine());
        _isStartGame = false;

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stop == false)
        {
            if (_isStartGame)
            {
                ForwardMovement();
                SideMovement();
            }
            
        }

    }

    void SizeControl()
    {
        if(playerDiamond.transform.localScale.x<=0.3f)
        {
            playerDiamond.transform.localScale = new Vector3(.3f, .3f, .3f);

        }
    }



    void SwipeControl()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                firstTouchPos = touch.position;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                deltaSwipeX = touch.position.x - firstTouchPos.x;

            }
           

        }

    }


    void SideMovement()
    {

        SwipeControl();
        if(Mathf.Abs(deltaSwipeX)>=10)
        {
            //transform.position = new Vector3((deltaSwipeX*0.005f), transform.position.y, transform.position.z);

            transform.position=Vector3.Lerp(transform.position, new Vector3((deltaSwipeX * 0.005f), transform.position.y, transform.position.z), .3f);
            
        }

        transform.position += new Vector3(Input.GetAxis("Horizontal") * swipeSpeed, 0, 0); // 0.003f ile çarpılacak

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.7f, 1.7f), transform.position.y, transform.position.z);
    }

    void ForwardMovement()
    {

        rb.velocity = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        modellerChild.transform.Rotate(0, 0, forwardSpeed * Time.fixedDeltaTime); //tekerlekDonusHizi ' ni forwardSpeed ile değiştirdim slider da farklılık oluyordu.

    }

    IEnumerator YuzukTaciAnimCoroutine()
    {
        while (true)
        {
            DOTween.Sequence()
                               .Append(tac.transform.DOScale(1.1f, 1f))
                               .Append(tac.transform.DOScale(1, 1f))
                               .SetEase(tacEase)
                               ;

            yield return new WaitForSeconds(2f);
        }


    }

    IEnumerator DoFinish()
    {
        //0.107, -0.92,15.5
        rb.isKinematic = true;
        DOTween.Sequence().Append(transform.DOMove(new Vector3(0.107f, 0.16f, 151.5f), 2f))
                          .Join(transform.DORotate(new Vector3(0, -90, 0), 2f))
                          .Append(transform.DOMove(new Vector3(0.107f, -.92f, 155.5f), 2f))
                          ;

        yield return new WaitForSeconds(4);
        UIManager.Instance.OpenFinishPanel();
    }
  



    void OnTriggerEnter(Collider other)
    {
        //////// Collecting 
        //RED Diamond hit
        if (other.gameObject.layer == 8)
        {
            // Toplama Sesi

            // Elmas büyüme
            if (diamondMat.color == redColor)
            {
                playerDiamond.transform.localScale += Vector3.one * _diamondSizeChangeAmount;
                playerDiamond.transform.position += new Vector3(0, 0.02f, 0);

                toplamaEfekti.Play();

            }
            else if (diamondMat.color != redColor)
            {
                playerDiamond.transform.localScale -= Vector3.one * _diamondSizeChangeAmount;
                playerDiamond.transform.position += new Vector3(0, -0.02f, 0);
                SizeControl();



            }


            Destroy(other.gameObject);

        }

        //Blue Diamond hit
        if (other.gameObject.layer == 9)
        {
            // Toplama Sesi

            // Elmas büyüme
            if (diamondMat.color == blueColor)
            {
                playerDiamond.transform.localScale += Vector3.one * _diamondSizeChangeAmount;
                playerDiamond.transform.position += new Vector3(0, 0.02f, 0);

                toplamaEfekti.Play();

            }
            if (diamondMat.color != blueColor)
            {
                playerDiamond.transform.localScale -= Vector3.one * _diamondSizeChangeAmount;
                playerDiamond.transform.position += new Vector3(0, -0.02f, 0);

                SizeControl();



            }

            Destroy(other.gameObject);

        }

        //Green Diamond hit
        if (other.gameObject.layer == 10)
        {
            // Toplama Sesi

            // Elmas büyüme
            if (diamondMat.color == greenColor)
            {
                playerDiamond.transform.localScale += Vector3.one * _diamondSizeChangeAmount;
                playerDiamond.transform.position += new Vector3(0, 0.02f, 0);

                toplamaEfekti.Play();

            }
            if (diamondMat.color != greenColor)
            {
                playerDiamond.transform.localScale -= Vector3.one * _diamondSizeChangeAmount;
                playerDiamond.transform.position += new Vector3(0, -0.02f, 0);

                SizeControl();



            }

            Destroy(other.gameObject);

        }


        //// Color Changing

        // Red Zone
        if (other.gameObject.CompareTag("redzone"))
        {
            Debug.Log("red");
            // Particle effect
            renkDegismeEfekti.Play();
            // Player Renk Değişimi
            diamondMat.color = redColor;
            // Ses

        }
        // Blue Zone
        if (other.gameObject.CompareTag("bluezone"))
        {
            //Particle effect
            renkDegismeEfekti.Play();

            // Player Renk Değişimi
            diamondMat.color = blueColor;

            // Ses

        }
        // Green Zone
        if (other.gameObject.CompareTag("greenzone"))
        {
            //Particle effect
            renkDegismeEfekti.Play();

            // Player Renk Değişimi
            diamondMat.color = greenColor;

            // Ses

        }

        // Finish
        if (other.gameObject.CompareTag("finish"))
        {

            stop = true;
            StartCoroutine(DoFinish());
            finishParticlesGameObject.SetActive(true);


        }


    }
}

