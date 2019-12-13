using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BlockSetting : MonoBehaviour
{
    //< 플레이어 오브젝트
    private GameObject player;

    //< 블럭프리팹 종류별로 배열에 집어넣기
    public GameObject[] road;

    //< 텍스쳐가 다르므로 도로의 끝에 들어갈 프리팹만 여기에
    public GameObject[] road_Ends;

    public GameObject[] grass;
    public GameObject[] water;
    public GameObject[] lava;
    public GameObject[] desert;
    public GameObject[] snow;
    public GameObject[] ash;

    public List<GameObject> blockList;

    //< 점수 텍스트
    public Text score;
    public Text max_score;

    //< 카메라에 비춰지는 최대 거리 설정
    public int cameraLength;

    //< 차, 뗏목의 속도 조절 값
    private const float MIN_SPEED = 0.008f;
    private const float MAX_SPEED = 0.15f;

    //< 연속으로 한 블록 타입이 생성 될 최대 개수
    private const int MAX_LENGTH = 8;

    //< 이후 형님께서 추가하셔도 됩니다.
    private enum BlockNames
    {
        GRASS = 0,
        WATER,
        LAVA,
        DESERT,
        SNOW,
        ASH,
        /*
         이 공간에 언제든지 추가 가능
         */
        ROAD,
        DummyData
    }

    //< 중첩 방지용
    private int randCount;
    private BlockNames checkDouble;

    //< z값 길이 설정용
    public int blockCount;

    //< 삭제 및 뒤로가기 방지용 거리체크
    //private int limitSetter = 0;

    //< 점수
    public int score_value = 0;
    public int max_score_value = 0;

    //< 플레이어 z값 갱신용
    //private int max_z;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];

        LoadFile();
        max_score.text = max_score_value.ToString();

        //< 절대 나올 수 없는 값을 설정
        randCount = 9999;
        checkDouble = BlockNames.DummyData;

        //< 풀은 기본설정(최소 5칸 = 뒤로 2칸도 있으므로 플레이어가 있는 블럭 포함 총 3~6칸 랜덤)
        int length = Random.Range(6, 9);

        for (int i = 0; i < 4; i++)
        {
            blockList.Add(Instantiate(grass[grass.Length - 1], new Vector3(0, -1, blockCount), Quaternion.identity));
            blockCount += 2;
        }

        //< 동적 생성
        GameObject p;
        for (int i = 0; i < length; i++)
        {
            int grass_setter = Random.Range(0, grass.Length - 3);

            while (randCount == grass_setter)
            {
                grass_setter = Random.Range(0, grass.Length - 3);
            }

            p = Instantiate(grass[grass_setter], new Vector3(0, 0, blockCount), Quaternion.identity);

            SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));

            blockList.Add(p);

            blockCount += 2;

            randCount = grass_setter;
        }
        //< 이후부터 모든 동적생성 블럭들은 랜덤으로
        //< 52까지인 이유는 50까지 생성되면 blockCount가 52가됨
        //< blockCount가 52여야 카메라에 빈공간이 잡히지 않으므로

        while (blockCount <= cameraLength)
        {
            MakeNewBlock();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (player != null)
        {
            //< 현재 블럭 생성될 위치 - 플레이어 z값 계산
            int playerZpos = blockCount - (int)player.transform.position.z;
            //int playerZpos = (int)player.transform.position.z;

            //< 형님 이거 제한 안걸면 생성될때는 여러개 생성되는데
            //< 지울때는 하나씩 지워서 순식간에 엄청나게 길어집니다
            //< max_z가 작아지면 동일하게 적용시킨다는거는 한번 넘어가면 바로 앞에 계속 생기는거라
            //< 엄청 길어집니다
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && (playerZpos <= cameraLength))
            {
                score_value++;

                //< 쓰지않는 변수인데 이거 왜 반환받으신검까?
                //int nBlocksGenerated = MakeNewBlock();
                MakeNewBlock();

                Destroy(blockList[0].gameObject);
                blockList.RemoveAt(0);

                GameObject.Find("Water").transform.Translate(0, 0, 2);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if ((GameObject.Find("Player_Coordinate").transform.position.z - GameObject.Find("Water").transform.position.z) > 4)
                {
                    Destroy(blockList[0].gameObject);
                    blockList.RemoveAt(0);

                    GameObject.Find("Water").transform.Translate(0, 0, 2);
                }
            }

            if (score_value >= max_score_value)
            {
                max_score.text = score_value.ToString();
                if (player.GetComponent<PlayerCtrl>().dead)
                {
                    SaveFile(score_value);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            score_value = 20;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            score_value = 40;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            score_value = 60;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            score_value = 80;
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            score_value = 100;
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            score_value = 120;
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            score_value = 140;
        }

        score.text = score_value.ToString();
    }

    //< 코드가 길어져서 좀 편하게 보려고 따로 함수화 해서 뺐습니다
    private void MakeNewBlock()
    {
        //< 지정된 enum문 활용
        BlockNames blocks = (BlockNames)Random.Range(0, System.Enum.GetValues(typeof(BlockNames)).Length - 1);

        //< 두번연속 떴을 경우만 즉시 반환하여 함수 탈출
        if (blocks == checkDouble)
        {
            //< 도로를 제외한 모든 경우의 수 중에서 선택
            return;
        }

        //< 생성될 블럭의 갯수(z값 길이)
        int randomLength = RandomNumberGenerator();

        //< 지정된 분기에 따라 생성
        switch (blocks)
        {
            case BlockNames.GRASS:
                GameObject p;
                for (int i = 0; i < randomLength; i++)
                {
                    int grass_setter = Random.Range(0, grass.Length - 1);

                    while (randCount == grass_setter)
                    {
                        grass_setter = Random.Range(0, grass.Length - 1);
                    }

                    p = Instantiate(grass[grass_setter], new Vector3(0, 0, blockCount), Quaternion.identity);
                    SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));
                    blockList.Add(p);

                    blockCount += 2;

                    randCount = grass_setter;
                }

                break;

            case BlockNames.WATER:

                for (int i = 0; i < randomLength; i++)
                {
                    int water_setter = Random.Range(0, water.Length);

                    while (randCount == water_setter)
                    {
                        water_setter = Random.Range(0, water.Length);
                    }

                    p = Instantiate(water[water_setter], new Vector3(0, 0, blockCount), Quaternion.identity);
                    SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));
                    blockList.Add(p);

                    blockCount += 2;

                    randCount = water_setter;
                }

                break;

            case BlockNames.LAVA:

                for (int i = 0; i < randomLength; i++)
                {
                    int lava_setter = Random.Range(0, lava.Length);

                    while (randCount == lava_setter)
                    {
                        lava_setter = Random.Range(0, lava.Length);
                    }

                    p = Instantiate(lava[lava_setter], new Vector3(0, 0, blockCount), Quaternion.identity);
                    SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));
                    blockList.Add(p);

                    blockCount += 2;

                    randCount = lava_setter;
                }

                break;

            case BlockNames.DESERT:

                for (int i = 0; i < randomLength; i++)
                {
                    int desert_setter = Random.Range(0, desert.Length);

                    while (randCount == desert_setter)
                    {
                        desert_setter = Random.Range(0, desert.Length);
                    }

                    p = Instantiate(desert[desert_setter], new Vector3(0, 0, blockCount), Quaternion.identity);
                    SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));
                    blockList.Add(p);

                    blockCount += 2;

                    randCount = desert_setter;
                }

                break;

            case BlockNames.SNOW:

                for (int i = 0; i < randomLength; i++)
                {
                    int snow_setter = Random.Range(0, snow.Length);

                    while (randCount == snow_setter)
                    {
                        snow_setter = Random.Range(0, snow.Length);
                    }

                    p = Instantiate(snow[snow_setter], new Vector3(0, 0, blockCount), Quaternion.identity);
                    SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));
                    blockList.Add(p);

                    blockCount += 2;

                    randCount = snow_setter;
                }

                break;

            case BlockNames.ASH:

                for (int i = 0; i < randomLength; i++)
                {
                    int ash_setter = Random.Range(0, ash.Length);

                    while (randCount == ash_setter)
                    {
                        ash_setter = Random.Range(0, ash.Length);
                    }

                    p = Instantiate(ash[ash_setter], new Vector3(0, 0, blockCount), Quaternion.identity);
                    SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));
                    blockList.Add(p);

                    blockCount += 2;

                    randCount = ash_setter;
                }

                break;

            case BlockNames.ROAD:

                for (int i = 0; i < randomLength; i++)
                {
                    int road_setter = Random.Range(0, road.Length);

                    while (randCount == road_setter)
                    {
                        road_setter = Random.Range(0, road.Length);
                    }

                    p = Instantiate(road[road_setter], new Vector3(0, 0, blockCount), Quaternion.identity);
                    SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));
                    blockList.Add(p);

                    blockCount += 2;

                    randCount = road_setter;
                }

                //< 도로의 마지막 1줄은 road_Ends에서 랜덤으로 하나 가져오기
                p = Instantiate(road_Ends[Random.Range(0, 1)], new Vector3(0, 0, blockCount), Quaternion.identity);
                SetSpeedForEachChild(p, Random.Range(MIN_SPEED, MAX_SPEED));
                blockList.Add(p);

                blockCount += 2;
                //randomLength++;
                break;
        }

        checkDouble = blocks;

        //< 정상작동하는지 디버깅용?
        //return randomLength;
    }

    //< Floor 오브젝트의 자식들중 차 또는 뗏목을 찾아서 속도를 랜덤하게 지정
    //< 단순히 게임씬의 모든 차, 뗏목 속도를 랜덤하게 하면 같은 라인에 있는 차들의 속도까지 달라지기 때문에
    //< 한 라인안에 있는 것들은 같은 값으로 일치 시킴
    private void SetSpeedForEachChild(GameObject p, float speed)
    {
        Transform t = p.transform;
        Transform c;

        for (int i = 0; i < t.childCount; i++)
        {
            c = t.GetChild(i);

            if (c.gameObject.tag == "Vehicle")
            {
                c.gameObject.GetComponent<EnemyCtrl>().MoveSpeed = speed;
            }
            else if (c.gameObject.tag == "Bridge")
            {
                c.gameObject.GetComponent<ObstacleCtrl>().MoveSpeed = speed;
            }
        }
    }

    private int RandomNumberGenerator()
    {
        int number = Random.Range(1, 101);
        int randomLength;

        if (number <= 50)
        {
            randomLength = Random.Range(1, 3);
        }
        else
        {
            if (number <= 80)
            {
                randomLength = Random.Range(3, 5);
            }
            else
            {
                if (number <= 95)
                {
                    randomLength = Random.Range(5, 7);
                }
                else
                {
                    randomLength = Random.Range(7, MAX_LENGTH + 1);
                }
            }
        }

        return randomLength;
    }

    public void SaveFile(int value)
    {
        string destination = Application.persistentDataPath + "/score.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, value);
        file.Close();
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