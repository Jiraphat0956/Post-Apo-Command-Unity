using UnityEngine;

public class UIPanel : MonoBehaviour
{
    // ฟังก์ชันพื้นฐานที่ทุก Panel ต้องมี
    public virtual void Show()
    {
        gameObject.SetActive(true);
        // คุณสามารถใส่ Logic การทำ Fade-in หรือเล่นเสียงกดปุ่มที่นี่ได้เลย
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    // ฟังก์ชันสำหรับ Initialize ข้อมูลครั้งแรก (ถ้ามี)
    public virtual void Setup() { }
}