using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Events;

using UnityEngine.Video;

using UnityEngine.UI;

public class VideoController : MonoBehaviour

{

    //视频部分

    public VideoPlayer vPlayer;

    public string urlNetWork = "D:\\b477d381a5fe878a28f229c691ab7eb1.mp4";//网络视频路径



    //视频控制器

    public Slider sliderVideo;

    //音量控制器

    public Slider sliderSource;

    //音量大小

    public Text text;

    //当前视频时间
    public Text text_Time;
    //视频总时长
    public Text text_Count;
    //音频组件
    public AudioSource source;
    //需要添加播放器的物体
    public GameObject obj;
    //是否拿到视频总时长
    public bool isShow;
    //前进后退的大小
    public float numBer = 20f;
    //时 分的转换
    private int hour, mint;
    private float time;
    private float time_Count;
    private float time_Current;
    //视频是否播放完成
    private bool isVideo;





    /////////

    void Start()

    {

        //一定要动态添加这两个组件，要不然会没声音

        vPlayer = obj.AddComponent<VideoPlayer>();

        source = obj.AddComponent<AudioSource>();



        //这3个参数不设置也会没声音 唤醒时就播放关闭

        vPlayer.playOnAwake = false;

        source.playOnAwake = false;

        source.Pause();



        //初始化

        Init(urlNetWork);

        ///////////////////视频初始化



        sliderSource.value = source.volume;

        text.text = string.Format("{0:0}%", source.volume * 100);

        sliderSource.onValueChanged.AddListener(delegate { ChangeSource(sliderSource.value); });//自动添加音频数据

    }



    // Update is called once per frame

    void Update()

    {

        if (vPlayer.isPlaying && isShow)

        {

            //帧数/帧速率=总时长    如果是本地直接赋值的视频，我们可以通过VideoClip.length获取总时长

            sliderVideo.maxValue = vPlayer.frameCount / vPlayer.frameRate;



            time = sliderVideo.maxValue;

            hour = (int)time / 60;

            mint = (int)time % 60;

            text_Count.text = string.Format("/  {0:D2}:{1:D2}", hour.ToString(), mint.ToString());



            sliderVideo.onValueChanged.AddListener(delegate { ChangeVideo(sliderVideo.value); });

            isShow = !isShow;

        }



        if (Mathf.Abs((int)vPlayer.time - (int)sliderVideo.maxValue) == 0)

        {

            vPlayer.frame = (long)vPlayer.frameCount;

            vPlayer.Stop();

            //Debug.Log("播放完成！");

            isVideo = false;

            return;

        }

        else if (isVideo)

        {

            time_Count += Time.deltaTime;

            if ((time_Count - time_Current) >= 1)

            {

                sliderVideo.value += 1;

                //Debug.Log("value:" + sliderVideo.value);

                time_Current = time_Count;

            }

        }

    }

    public void Video_Play(){
         vPlayer.Play();

         Time.timeScale = 1;
    }

    public void Video_Pause(){
         vPlayer.Pause();

         Time.timeScale = 0;
    }


    /// <summary>

    ///     初始化VideoPlayer

    /// </summary>

    /// <param name="url"></param>

    private void Init(string url)
    {

        isVideo = true;

        isShow = true;

        time_Count = 0;

        time_Current = 0;

        sliderVideo.value = 0;

        //设置为URL模式

        vPlayer.source = VideoSource.Url;

        //设置播放路径

        vPlayer.url = url;

        //在视频中嵌入的音频类型

        vPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;



        //把声音组件赋值给VideoPlayer

        vPlayer.SetTargetAudioSource(0, source);



        //当VideoPlayer全部设置好的时候调用

        vPlayer.prepareCompleted += Prepared;

        //启动播放器

        vPlayer.Prepare();

    }



    /// <summary>

    ///     改变音量大小

    /// </summary>

    /// <param name="value"></param>

    public void ChangeSource(float value)
    {

        source.volume = value;

        text.text = string.Format("{0:0}%", value * 100);

    }



    /// <summary>

    ///     改变视频进度

    /// </summary>

    /// <param name="value"></param>

    public void ChangeVideo(float value)
    {

        if (vPlayer.isPrepared)

        {

            vPlayer.time = (long)value;

            //Debug.Log("VideoPlayer Time:"+vPlayer.time);

            time = (float)vPlayer.time;

            hour = (int)time / 60;

            mint = (int)time % 60;

            text_Time.text = string.Format("{0:D2}:{1:D2}", hour.ToString(), mint.ToString());

        }

    }
    
    void Prepared(VideoPlayer player)
    {

        player.Play();

    }

}

