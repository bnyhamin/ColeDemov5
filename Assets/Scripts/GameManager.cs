using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int time = 30;
    public int difficulty = 1;
    public string[] cursos = null;
    public AudioClip[] audios;
    [SerializeField] int score;
    public GameObject pausePanel;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject prefabPanelDialog;
    [SerializeField] private GameObject panelStatus;
    [SerializeField] private GameObject VideoClase;
    [SerializeField] private GameObject[] ejercicios;
    [SerializeField] private GameObject[] examenes;
    [SerializeField] private GameObject material;
    [SerializeField] private GameObject _canvasCurso;
    [SerializeField] private GameObject panelContenido;
    [SerializeField] private GameObject goContenido;

    public GameObject panelEjercicios;
    public GameObject panelPracticas;
    public GameObject panelExamenes;
    public Text felicitaciones;
    public GameObject personajePanel;
    public GameObject[] personajes;
    [SerializeField] private GameObject _camCanvas;

    [SerializeField] private int idUsuario;
    [SerializeField] private string nombreUsuario;
    [SerializeField] private string correo;
    [SerializeField] private string dni;
    [SerializeField] private int puntaje;
    [SerializeField] private int idPersonaje;
    [SerializeField] private int grado;
    [SerializeField] private string nivel;
    [SerializeField] private bool _puntero_visible=true;
    [SerializeField] GameObject prefabButtonContenido;
    [SerializeField] GameObject _resonanceAudioDemoManager;
    [SerializeField] Camera[] cameras_personajes;

    [SerializeField] Text scoreText;
    /*public int respuestasExamenCorrectas=0;
    public int respuestasExamenIncorrectas=0;
    public Text txtCantCorrectas;
    public Text txtCantInorrectas;*/

    /*[SerializeField] private Material Red = null;
    [SerializeField] private Material Green = null;
    [SerializeField] private Material Yellow = null;
    [SerializeField] private Material Blue = null;*/

    public int Score
    {
        get => score;
        set
        {
            score = value;
            //UIManager.Instance.UpdateUIScore(score);

        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //UIManager.Instance.UpdateUIScore(score);
        //Time.timeScale = 1;
        
        idUsuario = PlayerPrefs.GetInt("varglobal_idUsuario");
        nombreUsuario = PlayerPrefs.GetString("varglobal_nombreUsuario");
        puntaje = PlayerPrefs.GetInt("varglobal_puntaje");
        correo = PlayerPrefs.GetString("varglobal_correo");
        idPersonaje = PlayerPrefs.GetInt("varglobal_idPersonaje");
        grado = PlayerPrefs.GetInt("varglobal_grado");
        nivel = PlayerPrefs.GetString("varglobal_nivel");

        //si no tiene personaje configurado abre panel para escoger personaje
        print("idUsuario:" + idUsuario);
        print("idPersonaje:" + idPersonaje);
        UIManager.Instance.UpdateUINombre(nombreUsuario);
        UIManager.Instance.UpdateUIScore();

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "SceneGame") return;
        if (idPersonaje == 0)
        {
            personajePanel.SetActive(true);
            _camCanvas.SetActive(true);
            panelStatus.SetActive(false);
        }
        else
        {
            choisePersonaje();
            _camCanvas.SetActive(false);
            panelStatus.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape"))
        {
            //SceneManager.LoadScene("Menu");
            //pausePanel.SetActive(true);
            
            PunteroVisible(_puntero_visible);

        }
    }

    public void PunteroVisible(bool visible)
    {
        print("visible:"+visible);
        if (visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            _camCanvas.SetActive(true);
            panelStatus.SetActive(true);
            _puntero_visible = false; //al gatillar cambia a este valor para la siguiente vez
            cleanPersonaje();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
            _camCanvas.SetActive(false);
            panelStatus.SetActive(true);
            _puntero_visible = true;//al gatillar cambia a este valor para la siguiente vez
            choisePersonaje();
        }


    }

    public void PunteroVisiblePizarra()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelStatus.SetActive(false);
        _puntero_visible = true;
    }

    public void startTime()
    {
        //GameObject.Find("Time").SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(1);
        /*while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            UIManager.Instance.UpdateUITime(time);
        }
        if (time <= 0)
        {
            UIManager.Instance.showUITime(false);
        }*/
    }

    public void Unpause()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Ajustes()
    {
        Debug.Log("Ajustes");
        personajePanel.SetActive(true);
        //_camCanvas.SetActive(true);
    }


    public void ChangePersonaje(int id_personaje)
    {
        StartCoroutine(Change_Personaje(id_personaje));
    }

    IEnumerator Change_Personaje(int id_personaje)
    {
        idPersonaje = id_personaje;
        WWW conneccion = new WWW("https://virtual.espinosa.edu.pe/proyecto/espinosa3d/actualizar_personaje.php?id_usuario=" + idUsuario + "&id_personaje=" + idPersonaje);
        yield return (conneccion);
        if (conneccion.text == "201")
        {
            personajePanel.SetActive(false);
            choisePersonaje();
            print("registr� correctamente");
        }
        else
        {
            Debug.LogError("Error en la conecci�n con la base de datos");
        }
    }

    public void cleanPersonaje()
    {
        for(int i = 0; i < personajes.Length; i++)
        {
            personajes[i].SetActive(false);
        }
        //GameObject.Find("Man").SetActive(false);
        //GameObject.Find("Woman").SetActive(false);
    }

    
    public void choisePersonaje()
    {        
        cleanPersonaje();
        if (!_puntero_visible)
        {   
            Time.timeScale = 1;
            _camCanvas.SetActive(false);
            panelStatus.SetActive(true);
            _puntero_visible = true;
        }
            _camCanvas.SetActive(false);
        if (idPersonaje == 1) personajes[0].SetActive(true);
        if (idPersonaje == 2) personajes[1].SetActive(true);
        //_resonanceAudioDemoManager.GetComponent<ResonanceAudioDemoManager>().mainCamera = cameras_personajes[1];
        _resonanceAudioDemoManager.GetComponent<ResonanceAudioDemoManager>().mainCamera = (idPersonaje == 1)?cameras_personajes[0]: cameras_personajes[1];
        //_resonanceAudioDemoManager.GetComponent<ResonanceAudioDemoManager>().mainCamera = GameObject.Find("Main CameraWoman").gameObject.GetComponent<Camera>();
        //(idPersonaje == 1 ? GameObject.Find("Main CameraMan") : GameObject.Find("Main CameraWoman"));

    }

    public void activecameraDJ()
    {
        personajes[0].SetActive(false);
        personajes[1].SetActive(false);
        personajes[2].SetActive(true);
        _resonanceAudioDemoManager.GetComponent<ResonanceAudioDemoManager>().mainCamera = cameras_personajes[2];
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        _puntero_visible = true;
    }

    public void showLivingInformation(bool fgvisible, string text)
    {        
        string mensaje = string.Empty;
        if(text == "ColliderAula1")
        {
            mensaje = "Bienvenido al Aula de Historia!." +
                "                          " +
                "En este curso conocer�s la Historia del Per� y del Mundo";
        }
        prefabPanelDialog.SetActive(fgvisible);
        UIManager.Instance.ChangeDialog(mensaje);
        StartCoroutine(WaitingInClass(2));
    }

    IEnumerator WaitingInClass(int time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("SceneSalon");
    }


    public void Logout()
    {
        Debug.Log("Logout");
        Time.timeScale = 1;
        SceneManager.LoadScene("SceneLogin");
    }

    public void OpenVideoClases()
    {
        //Debug.Log("Activar Videoclases");
        //panelVideoClase.SetActive(true);
        panelStatus.SetActive(false);
        GameObject video = Instantiate(VideoClase);
        //video.transform = assetRoot.transform.position;
        //Instantiate(VideoClase, new Vector3(0, 0, 0), Quaternion.identity);
        video.transform.parent = canvas.transform;
    }


    public void OpenEjercicios()
    {
        panelEjercicios.SetActive(true);
        int id_contenido = PlayerPrefs.GetInt("varglobal_idContenido");
        //string _path = Application.dataPath + "/Sprites/Clases/Contenido/" + idcontenido + "/ejercicio.prefab";
        //GameObject ejercicio = (GameObject)AssetDatabase.LoadMainAssetAtPath(_path);
        //GameObject video = Instantiate(VideoClase);        
        //video.transform.parent = canvas.transform;
        //string relativePath = "https://virtual.espinosa.edu.pe/proyecto/espinosa3d/Contenido/" + id_contenido + "/ejercicio";
        //string relativePath = Application.dataPath + "/Sprites/Clases/Contenido/" + idcontenido + "/ejercicio";
        //material = Resources.Load<GameObject>(relativePath);
        /*print("busca material "+ material);
        if (material != null)
        {
            print("entra a instanciar material");
            GameObject objectButton = Instantiate(material);
            //objectButton.transform.SetParent(panelEjercicios.transform);
            objectButton.transform.parent = panelEjercicios.transform;
        }*/
        if(id_contenido == 6)
        {
            ejercicios[0].gameObject.SetActive(true);
        }
        if (id_contenido == 10)
        {
            ejercicios[1].gameObject.SetActive(true);
        }

        //GameObject objectButton = Instantiate(prefabButtonContenido);
        //objectButton.transform.parent = panelContenido.transform;



    }

    public void OpenExamen()
    {
        panelExamenes.SetActive(true);
        int id_contenido = PlayerPrefs.GetInt("varglobal_idContenido");
        //string _path = Application.dataPath + "/Sprites/Clases/Contenido/" + idcontenido + "/ejercicio.prefab";
        //GameObject ejercicio = (GameObject)AssetDatabase.LoadMainAssetAtPath(_path);
        //GameObject video = Instantiate(VideoClase);        
        //video.transform.parent = canvas.transform;
        if (id_contenido == 7)
        {
            examenes[0].gameObject.SetActive(true);
        }

        if (id_contenido == 11)
        {
            examenes[1].gameObject.SetActive(true);
        }

        //GameObject objectButton = Instantiate(prefabButtonContenido);
        //objectButton.transform.parent = panelContenido.transform;



    }

    public void CloseMaterial(int id_tipo_contenido)
    {
        if (id_tipo_contenido == 2) panelEjercicios.SetActive(false);
        if (id_tipo_contenido == 3) panelPracticas.SetActive(false);
        if (id_tipo_contenido == 5) panelExamenes.SetActive(false);
    }

    public void CloseExamen()
    {
        panelExamenes.SetActive(false);
    }

    public void envia_diploma(int id_contenidoactual)
    {
        StartCoroutine(envio_diploma(id_contenidoactual));
    }

    IEnumerator envio_diploma(int idcontenido)
    {
    
        WWW conneccion = new WWW("https://virtual.espinosa.edu.pe/proyecto/espinosa3d/Lanzador/Scripts/generar_imagenes.php?id_usuario=" + idUsuario + "&id_contenido=" + idcontenido);
        print("https://virtual.espinosa.edu.pe/proyecto/espinosa3d/Lanzador/Scripts/generar_imagenes.php?id_usuario=" + idUsuario + "&id_contenido=" + idcontenido);
        yield return (conneccion);
        print("Enviado");
    }


    /*public void CloseVideoClases()
    {
        //Debug.Log("Activar Videoclases");
        //panelVideoClase.SetActive(false);
        _canvasCurso.SetActive(false);
        panelStatus.SetActive(true);

    }*/
    private int idcontenidoNuevo;
    /*public void actualizar_contenidoultimovistocursorepetido(int idcontenidoactual)
    {
        int id_curso = PlayerPrefs.GetInt("varglobal_idCurso");

        StartCoroutine(actualizar_puntaje((resultado) =>
        {
            idcontenidoNuevo = resultado;
            Debug.Log("El resultado obtenido es: " + idcontenidoNuevo);
        }, idcontenidoactual));
        //int idcontenido = int.Parse(StartCoroutine(actualizar_puntaje(idcontenidoactual)));
        int idcontenido = idcontenidoNuevo;
        print("idcontenido es :" + idcontenido);

        varglobal_idContenido

        PlayerPrefs.SetInt("varglobal_idContenidoUltimoVistoCurso", idcontenido);

       
    }*/

    
    public void actualizar_contenidoultimovistocurso(int idcontenidoactual)
    {
        int id_curso = PlayerPrefs.GetInt("varglobal_idCurso");
        //int idcontenido = int.Parse(StartCoroutine(actualizar_puntaje(idcontenidoactual)));
        StartCoroutine(actualizar_puntaje((resultado) =>
        {
            //idcontenidoNuevo = resultado;
            //Debug.Log("El resultado obtenido es: " + idcontenidoNuevo);


            int idcontenido = resultado;
            print("idcontenido es :" + idcontenido);

            PlayerPrefs.SetInt("varglobal_idContenidoUltimoVistoCurso", idcontenido);

            string contenidos_actuales = PlayerPrefs.GetString("varglobal_contenidos_actuales");
            string[] contenidos = contenidos_actuales.Split(",");
            string contenidosactualizar = string.Empty;
            print("Actualizando contenidoultimovistocurso| id_curso:" + id_curso + ":idcontenido:" + idcontenido);
            for (int i = 0; i < contenidos.Length; i++)
            {
                print("contenidosactualizar:[" + i + "]| id_curso:" + id_curso + "-contenidos[i].Split(:)[0]):" + int.Parse(contenidos[i].Split(":")[0]));
                if (id_curso == int.Parse(contenidos[i].Split(":")[0]))
                {
                    PlayerPrefs.SetInt("varglobal_idContenidoUltimoVistoCurso", idcontenido);
                    contenidosactualizar = (contenidosactualizar != "" ? contenidosactualizar + "," : "") + id_curso + ":" + idcontenido;
                    print("Asignando a la memoria contenido:" + idcontenido);
                    PlayerPrefs.SetInt("varglobal_idContenido", idcontenido);
                    //print("varglobal_idContenidoUltimoVistoCurso:" + PlayerPrefs.GetInt("varglobal_idContenidoUltimoVistoCurso"));
                }
                else
                {
                    contenidosactualizar = (contenidosactualizar != "" ? contenidosactualizar + "," : "") + contenidos[i].Split(":")[0] + ":" + contenidos[i].Split(":")[1];


                }
                print("contenidosactualizar:[" + i + "]:" + contenidosactualizar);
            }
            PlayerPrefs.SetString("varglobal_contenidos_actuales", contenidosactualizar);
            StartCoroutine(actualizarcontenidos_actuales(contenidosactualizar, idcontenido));






        }, idcontenidoactual));
        
        
    }

    IEnumerator actualizar_puntaje(System.Action<int> callback, int idcontenido)
    {

        WWW conneccion = new WWW("https://virtual.espinosa.edu.pe/proyecto/espinosa3d/actualizar_puntaje.php?id_usuario=" + idUsuario + "&id_contenido=" + idcontenido );
        yield return (conneccion);
        print("DEBUG1 conneccion.text:" + conneccion.text);
        if (conneccion.text == "401")
        {
            print("ID Contenidos_actuales incorrecto");
            
        }
        else
        {
            int id_contenido_next = int.Parse(conneccion.text.Split("|")[0]);
            int puntaje_actualizar = int.Parse(conneccion.text.Split("|")[1]);
            print("DEBUG2  id_contenido_next:" + id_contenido_next+ "|puntaje_actualizar:"+ puntaje_actualizar);
            PlayerPrefs.SetInt("varglobal_puntaje", puntaje_actualizar);
            UIManager.Instance.UpdateUIScore();
            //yield return (id_contenido_next);
            callback(id_contenido_next);
        }
    }

    IEnumerator actualizarcontenidos_actuales(string contenidosactualizar, int idcontenido)
    {

        WWW conneccion = new WWW("https://virtual.espinosa.edu.pe/proyecto/espinosa3d/actualizar_contenidosactuales.php?id_usuario=" + idUsuario + "&id_contenido="+ idcontenido + "&contenidos_actuales=" + contenidosactualizar);
        yield return (conneccion);
        print("DEBUG1 conneccion.text:" + conneccion.text + "--actualizar_contenidosactuales.php?id_usuario=" + idUsuario + "&id_contenido=" + idcontenido + "&contenidos_actuales=" + contenidosactualizar);
        if (conneccion.text == "401")
        {
            print("ID Contenidos_actuales incorrecto");
            //textError.text = "Usuario incorrecto";
            //MostrarError("Usuario incorrecto");
        }
        else
        {
            string texto = conneccion.text;
            if (texto == "401" || texto =="")
            {
                print("obteniendo   id_tipo_contenido, error 401:" + texto);
                yield return (conneccion);
            }

            int id_tipo_contenido = int.Parse(conneccion.text);
            
            //int id_tipo_contenido = int.Parse(conneccion.text.Split("|")[0]);
            //int puntaje = int.Parse(conneccion.text.Split("|")[1]);
            print("DEBUG2  id_tipo_contenido:" + id_tipo_contenido);
            //PlayerPrefs.SetInt("varglobal_puntaje", puntaje);
            //UIManager.Instance.UpdateUIScore();
            
            if (id_tipo_contenido==1)
            {
                //OpenVideoClases();
                panelStatus.SetActive(false);
                GameObject video = Instantiate(VideoClase);                
                video.transform.parent = canvas.transform;
            }
            if (id_tipo_contenido == 2)
            {
                /*string absoluteURL = "https://virtual.espinosa.edu.pe/proyecto/espinosa3d/Contenido/" + idcontenido + "/ejercicio.prefab";
                UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(absoluteURL);
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error al descargar el AssetBundle: " + www.error);
                    yield break;
                }

                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                GameObject prefab = bundle.LoadAsset<GameObject>("ejercicio");
                if (prefab != null)
                {
                    Instantiate(prefab, panelEjercicios.transform);
                }
                else
                {
                    Debug.LogError("No se pudo cargar el Prefab desde el AssetBundle.");
                }

                bundle.Unload(false);
                */
                panelEjercicios.SetActive(true);
                string relativePath = "Sprites/Clases/Contenido/" + idcontenido + "/ejercicio";               
                //string relativePath = Application.dataPath + "/Sprites/Clases/Contenido/" + idcontenido + "/ejercicio";
                print(relativePath);
                material = Resources.Load<GameObject>(relativePath);                
                if (material != null)
                {

                    GameObject objectButton = Instantiate(material);
                    objectButton.transform.parent = panelEjercicios.transform;
                }


                /* 
                panelEjercicios.SetActive(true);
               

                //string relativePath = "Sprites/Clases/Contenido/" + idcontenido + "/ejercicio";
                string relativePath = "https://virtual.espinosa.edu.pe/proyecto/espinosa3d/Contenido/" + idcontenido + "/ejercicio";
                //string relativePath = Application.dataPath + "/Sprites/Clases/Contenido/" + idcontenido + "/ejercicio";
                material = Resources.Load<GameObject>(relativePath);
                print("Primer material" + material);
                if (material != null)
                {
                    
                    GameObject objectButton = Instantiate(material);
                    //objectButton.transform.SetParent(panelEjercicios.transform);
                    objectButton.transform.parent = panelEjercicios.transform;
                }
                */



            }
            if (id_tipo_contenido == 3)
            {
                //OpenExamen();
                panelPracticas.SetActive(true);
                string relativePath = "Sprites/Clases/Contenido/" + idcontenido + "/practica";
                //string relativePath = "https://virtual.espinosa.edu.pe/proyecto/espinosa3d/Contenido/" + idcontenido + "/practica";
                material = Resources.Load<GameObject>(relativePath);
                if (material != null)
                {

                    GameObject objectButton = Instantiate(material);
                    objectButton.transform.SetParent(panelPracticas.transform);
                    //objectButton.transform.parent = panelEjercicios.transform;
                }

            }

            if (id_tipo_contenido == 5)
            {
                //OpenExamen();
                panelExamenes.SetActive(true);
                string relativePath = "Sprites/Clases/Contenido/" + idcontenido + "/examen";
                //string relativePath = "https://virtual.espinosa.edu.pe/proyecto/espinosa3d/Contenido/" + idcontenido + "/examen";
            
                material = Resources.Load<GameObject>(relativePath);
                if (material != null)
                {

                    GameObject objectButton = Instantiate(material);
                    objectButton.transform.SetParent(panelExamenes.transform);
                    //objectButton.transform.parent = panelEjercicios.transform;
                }
                //emite certificado
            }



            /*if (id_tipo_contenido == 5)
            {
                OpenExamenFinal();
            }*/
        }

    }


    public void abrirCurso(int id_curso)
    {
        //panelVideoClase.SetActive(false);
        _canvasCurso.SetActive(true);
        print("ENTRO AL CURSO" + id_curso);
        PlayerPrefs.SetInt("varglobal_idCurso", id_curso);
        string contenidos_actuales =PlayerPrefs.GetString("varglobal_contenidos_actuales");
        string[] contenidos = contenidos_actuales.Split(",");
        print("contenidos_actuales:" + contenidos_actuales);
        for (int i=0; i< contenidos.Length; i++)
        {
            print("contenido_curso:["+i+"]:"+contenidos[i].Split(":")[0]);
            if (id_curso == int.Parse(contenidos[i].Split(":")[0]) )
            {
                PlayerPrefs.SetInt("varglobal_idContenidoUltimoVistoCurso", int.Parse(contenidos[i].Split(":")[1]));
                PlayerPrefs.SetInt("varglobal_idContenido", int.Parse(contenidos[i].Split(":")[1]));
                //print("varglobal_idContenidoUltimoVistoCurso:" + PlayerPrefs.GetInt("varglobal_idContenidoUltimoVistoCurso"));
            }
        }
    }

    public void abrirLienzo()
    {
        panelContenido.SetActive(true);
        //llamar a corrutina para que despliegue
        StartCoroutine(contenidos(PlayerPrefs.GetInt("varglobal_idCurso"))); 
    }

    public void seleccionContenido(int idcontenido)
    {
        print("Asignando a la memoria contenido:" + idcontenido);
        PlayerPrefs.SetInt("varglobal_idContenido", idcontenido);
        panelContenido.SetActive(false);

    }


    IEnumerator contenidos(int id_curso)
    {

        WWW conneccion = new WWW("https://virtual.espinosa.edu.pe/proyecto/espinosa3d/contenidos.php?id_curso=" + id_curso);
        yield return (conneccion);
        print("DEBUG conneccion.text:"+conneccion.text);
        if (conneccion.text == "401")
        {
            print("ID Curso incorrecto");
            //textError.text = "Usuario incorrecto";
            //MostrarError("Usuario incorrecto");
        }
        else
        {

            print("substring:" + conneccion.text);
            string[] nDatos = conneccion.text.Substring(0, conneccion.text.Length - 1).Split("-");
            GameManager.Instance.cursos = nDatos;
            RectTransform _rectransform;
            int j = 0;
            for (float i = 0; i < nDatos.Length; i++)
            {

                GameObject objectButton = Instantiate(prefabButtonContenido);
                objectButton.transform.parent = goContenido.transform;//panelContenido.transform; //panelContenido.GetComponent<ScrollRect>().content;
                _rectransform = objectButton.GetComponent<RectTransform>();
                _rectransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                _rectransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                _rectransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                _rectransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);

                _rectransform.anchorMin = new Vector2(0.02f, (0.9f - (i / 10)));
                _rectransform.anchorMax = new Vector2(1.0f, (1.0f -  (i / 10)));
                objectButton.transform.GetChild(0).GetComponent<Text>().text = nDatos[j].ToString().Split("|")[2];
                objectButton.transform.GetChild(0).GetComponent<Text>().resizeTextForBestFit = true;
                objectButton.transform.GetChild(0).GetComponent<Text>().font.name = "Arial";

                //objectButton.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.abrirCurso(nDatos[j].ToString().Split("|")[0]));
                int id_contenido = int.Parse(nDatos[j].Split("|")[0]);
                print("ID_CONTENIDO:" + id_contenido);
                if (id_contenido == PlayerPrefs.GetInt("varglobal_idContenidoUltimoVistoCurso")){
                    objectButton.GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
                }
                else
                {
                    objectButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
                } 

                objectButton.GetComponent<Button>().onClick.AddListener(() => gameObject.GetComponent<GameManager>().seleccionContenido(id_contenido));
                


                j++;
            }



        }

    }

    /*public void UpdateTextCantidades()
    {
        print("Resp correctas:"+ respuestasExamenCorrectas.ToString());
        print("Resp INcorrectas:" + respuestasExamenIncorrectas.ToString());
        txtCantCorrectas.text = respuestasExamenCorrectas.ToString();
        txtCantInorrectas.text = respuestasExamenIncorrectas.ToString();
    }*/

    /*public void UpdateUIScore()
    {
        int puntaje = PlayerPrefs.GetInt("varglobal_puntaje");
        print("Ingresando puntaje:" + puntaje);
        //scoreText.text = puntaje.ToString();
    }*/




}
