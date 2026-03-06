using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class IA_Citoyen : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;

    [Header("Réglages Temps (Secondes)")]
    public float tempsDAttenteMin = 3f;  // Modifié selon ta demande
    public float tempsDAttenteMax = 5f;  // Modifié selon ta demande
    public float marcheMin = 5f;         // Nouvelle variable
    public float marcheMax = 10f;        // Nouvelle variable

    private float _chronoMarche = 0f;
    private float _limiteActuelle;       // Stocke le tirage aléatoire pour le trajet en cours
    private bool _estEnTrainDeMarcher = false;

    [Header("Animations Aléatoires")]
    public int nombreAnimationsSpeciales = 6;
    [Range(0, 100)]
    public float chanceAnimationSpeciale = 25;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        // Initialisation de la première limite
        _limiteActuelle = Random.Range(marcheMin, marcheMax);
        AllerADestinationAleatoire();
    }

    void Update()
    {
        float vitesseActuelle = _agent.velocity.magnitude;
        _animator.SetFloat("Vitesse", vitesseActuelle);

        // Gestion du chronomètre
        if (_agent.hasPath && _agent.remainingDistance > _agent.stoppingDistance)
        {
            _estEnTrainDeMarcher = true;
            _chronoMarche += Time.deltaTime;
        }
        else
        {
            _estEnTrainDeMarcher = false;
            _chronoMarche = 0f;
        }

        // CONDITION 1 : Arrivée normale
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!IsInvoking("AllerADestinationAleatoire") && _animator.GetInteger("AnimationID") == 0)
            {
                GererAttente();
            }
        }

        // CONDITION 2 : Limite de temps atteinte (recalculée à chaque départ)
        if (_chronoMarche >= _limiteActuelle)
        {
            _agent.ResetPath();
            _chronoMarche = 0f;
            GererAttente();
        }
    }

    void GererAttente()
    {
        if (Random.Range(0f, 100f) <= chanceAnimationSpeciale)
        {
            int idAleatoire = Random.Range(1, nombreAnimationsSpeciales + 1);
            _animator.SetInteger("AnimationID", idAleatoire);
            StartCoroutine(ResetAnimationID());
        }

        float attente = Random.Range(tempsDAttenteMin, tempsDAttenteMax);
        Invoke("AllerADestinationAleatoire", attente);
    }

    IEnumerator ResetAnimationID()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetInteger("AnimationID", 0);
    }

    void AllerADestinationAleatoire()
    {
        // On définit la limite pour CE trajet précis
        _limiteActuelle = Random.Range(marcheMin, marcheMax);

        Vector3 destination = ObtenirPointAleatoireSurNavMesh(transform.position, 100f);
        _agent.SetDestination(destination);
        _chronoMarche = 0f;
    }

    Vector3 ObtenirPointAleatoireSurNavMesh(Vector3 centre, float distance)
    {
        Vector3 pointHasard = centre + Random.insideUnitSphere * distance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(pointHasard, out hit, distance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return centre;
    }
}