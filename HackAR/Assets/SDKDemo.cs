using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monaverse;
using Monaverse.Core;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Monaverse.Api;
using Mona.SDK.Brains.Core.Utils.Structs;
using MonaDemo;

public class SDKDemo : MonoBehaviour
{
    private string _walletAddress;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Connect", 3f);
        //Debug.Log("tokens");
    }

    // Update is called once per frame
    async void Connect()
    {
        await MonaverseManager.Instance.SDK.Disconnect();

        await Task.Delay(100);

        _walletAddress = await MonaverseManager.Instance.SDK.ConnectWallet();

        Debug.Log($"{nameof(Connect)} {_walletAddress}");

        if (MonaverseManager.Instance.SDK.IsWalletAuthorized())
        {

        }

        else
        {
            var messageResponse = await MonaverseManager.Instance.SDK.AuthorizeWallet();

            if(messageResponse == MonaWalletSDK.AuthorizationResult.Authorized)
            {
                var response = await MonaApi.ApiClient.Collectibles.GetWalletCollectibles();

                if (response.IsSuccess)
                {
                    var tokens = response.Data.Data;

                    var token = tokens.Find(x =>
                    {
                        return x.Type == "Artifact";
                    });
                    var url = token.Versions[token.ActiveVersion].Asset;

                    //var url = token.Versions[token.ActiveVersion].Asset;

                    Debug.Log($"tokens {token}");
                    

                    var loader = GetComponent<GlbLoader>();
                    loader.Load(url, true, (GameObject obj) =>
                    {
                        obj.transform.position = new Vector3(-100f, -100f, -100f);
                        obj.transform.rotation = Quaternion.Euler(0, 180f, 0);

                        // Check if the loaded object has children
                        if (obj.transform.childCount > 0)
                        {
                            // Get the first child object
                            GameObject childObj = obj.transform.GetChild(0).gameObject;

                            // Get the MeshFilter of the child object
                            MeshFilter childMeshFilter = childObj.GetComponent<MeshFilter>();

                            if (childMeshFilter != null)
                            {
                                // Find all GameObjects with the tag "Persistent"
                                GameObject[] persistents = GameObject.FindGameObjectsWithTag("Persistent");

                                foreach (GameObject persistent in persistents)
                                {
                                    // Get or add MeshFilter component to the persistent object
                                    MeshFilter persMeshFilter = persistent.GetComponent<MeshFilter>();
                                    if (persMeshFilter == null)
                                    {
                                        persMeshFilter = persistent.AddComponent<MeshFilter>();
                                    }

                                    // Get or add Renderer component to the persistent object
                                    Renderer persRenderer = persistent.GetComponent<Renderer>();
                                    if (persRenderer == null)
                                    {
                                        persRenderer = persistent.AddComponent<MeshRenderer>(); // Assuming you want MeshRenderer
                                    }

                                    // Replace the mesh of the persistent object with the child's mesh
                                    persMeshFilter.mesh = childMeshFilter.mesh;

                                    // Copy materials from the child object to the persistent object
                                    Renderer childRenderer = childObj.GetComponent<Renderer>();
                                    if (childRenderer != null && persRenderer != null)
                                    {
                                        persRenderer.sharedMaterials = childRenderer.sharedMaterials;
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Child object or persistent object does not have a Renderer component.");
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogWarning("The child object does not have a MeshFilter component.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("The loaded object does not have any children.");
                        }
                    });
                }

                
            }

            
        }
    }

    
}
