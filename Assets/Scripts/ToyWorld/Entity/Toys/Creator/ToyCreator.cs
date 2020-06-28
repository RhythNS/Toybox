using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class for spawning a randomized Toy
/// </summary>
public class ToyCreator : MonoBehaviour
{
    public static ToyCreator Instance { get; private set; }

    [SerializeField] private CreatorHelper haonUnityChanPrefab;

    [SerializeField] private Material[] accessory01;
    [SerializeField] private Material[] accessory00And02;

    [SerializeField] private MultiMaterial[] costumes;

    [SerializeField] private MultiMaterial[] eyes;

    [SerializeField] private Material[] eyeBrow;
    [SerializeField] private MultiMaterial[] hair;

    [SerializeField] private Material[] faceSkin;

    [System.Serializable]
    public class MultiMaterial
    {
        public Material[] materials;
    }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Generates a toy on a given toori.
    /// </summary>
    /// <param name="toori"></param>
    /// <returns></returns>
    public Toy Generate(Toori toori, string name)
    {
        CreatorHelper creatorHelper = Instantiate(haonUnityChanPrefab, toori.Town.transform);
        creatorHelper.gameObject.layer = 8;

        if (Random.value > 0.4) // 60% chance to spawn with an accessory
        {
            int accessoryNumber = Random.Range(0, 3);
            SkinnedMeshRenderer accessoryRenderer;
            Material accessoryMaterial;
            if (accessoryNumber == 1)
            {
                accessoryRenderer = creatorHelper.accessory01;
                accessoryMaterial = RandomMaterial(accessory01);
            }
            else
            {
                accessoryRenderer = accessoryNumber == 0 ? creatorHelper.accessory00 : creatorHelper.accessory02;
                accessoryMaterial = RandomMaterial(accessory00And02);
            }
            accessoryRenderer.gameObject.SetActive(true);
            accessoryRenderer.material = accessoryMaterial;
        }

        // Get a random eye
        int eyeNumber = Random.Range(0, creatorHelper.eyes.Length);
        SkinnedMeshRenderer eyeRenderer = creatorHelper.eyes[eyeNumber];
        Material eyeMaterial = RandomMaterial(eyes[eyeNumber].materials);
        eyeRenderer.gameObject.SetActive(true);
        eyeRenderer.material = eyeMaterial;

        // Get random hair
        int hairType = Random.Range(0, hair.Length);
        int hairNumber = Random.Range(0, hair[hairType].materials.Length);
        CreatorHelper.MultiSkinnedRenderer hairs = creatorHelper.hairs[hairType];
        Material hairMaterial = hair[hairType].materials[hairNumber];
        for (int i = 0; i < hairs.renderers.Length; i++)
        {
            hairs.renderers[i].gameObject.SetActive(true);
            hairs.renderers[i].material = hairMaterial;
        }

        // get random eyebrow and face
        Material eyeBrowMaterial = eyeBrow[hairNumber];
        Material faceMaterial = RandomMaterial(faceSkin);

        Material[] faceMaterialArray = { faceMaterial, eyeBrowMaterial, eyeBrowMaterial };

        SkinnedMeshRenderer[] faces = creatorHelper.faces;
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].materials = faceMaterialArray;
        }
        faces[0].gameObject.SetActive(true);

        // get random costume
        int costumeType = Random.Range(0, costumes.Length);
        CreatorHelper.MultiSkinnedRenderer costumeRenderers = creatorHelper.costumes[costumeType];
        Material costumeMaterial = RandomMaterial(costumes[costumeType].materials);
        costumeRenderers.renderers[0].gameObject.SetActive(true);
        costumeRenderers.renderers[0].material = costumeMaterial;
        costumeRenderers.renderers[1].gameObject.SetActive(true);
        costumeRenderers.renderers[1].material = faceMaterial;

        // finaly spawn the Toy and destroy this helper class
        Toy toy = creatorHelper.gameObject.GetComponent<Toy>();
        toy.Town = toori.Town;
        toy.name = name;
        Destroy(creatorHelper);

        toy.GetComponent<NavMeshAgent>().Warp(toori.EnterPosition.position);

        return toy;
    }

    private Material RandomMaterial(Material[] material) => material[Random.Range(0, material.Length)];

}
