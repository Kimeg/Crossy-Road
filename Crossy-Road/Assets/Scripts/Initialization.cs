using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    private int max_score_value = 0;
    private const int scaler = 20;

    public GameObject chicken;
    public GameObject dog;
    public GameObject fox;
    public GameObject pig;
    public GameObject lion;
    public GameObject cow;
    public GameObject penguin;
    public GameObject cat;

    // Start is called before the first frame update
    private void Awake()
    {
        LoadFile();

        if (max_score_value < scaler)
        {
            Instantiate(chicken, Vector3.zero, Quaternion.identity);
        }
        else
        {
            if (max_score_value < 2 * scaler)
            {
                Instantiate(cat, Vector3.zero, Quaternion.identity);
            }
            else
            {
                if (max_score_value < 3 * scaler)
                {
                    Instantiate(dog, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    if (max_score_value < 4 * scaler)
                    {
                        Instantiate(fox, Vector3.zero, Quaternion.identity);
                    }
                    else
                    {
                        if (max_score_value < 5 * scaler)
                        {
                            Instantiate(pig, Vector3.zero, Quaternion.identity);
                        }
                        else
                        {
                            if (max_score_value < 6 * scaler)
                            {
                                Instantiate(lion, Vector3.zero, Quaternion.identity);
                            }
                            else
                            {
                                if (max_score_value < 7 * scaler)
                                {
                                    Instantiate(cow, Vector3.zero, Quaternion.identity);
                                }
                                else
                                {
                                    Instantiate(penguin, Vector3.zero, Quaternion.identity);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/score.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        max_score_value = (int)bf.Deserialize(file);
        file.Close();
    }
}