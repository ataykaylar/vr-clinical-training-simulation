
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class CableProceduralCurve : MonoBehaviour
{

    LineRenderer line;

    [SerializeField] Transform endPointTransform;

    [SerializeField, Tooltip("Number of points per unit length, using the straight line from the start to the end transform.")] float pointDensity = 3;

    [SerializeField, Tooltip("Positive keys are applied downward on cable. Recommended use is to have keys at 0 at the start and end of the curve.")] AnimationCurve curve;

    // Bu değerlerin artık Start'ta tanımlanmasına gerek yok
    int pointsInLineRenderer;
    Vector3 vectorFromStartToEnd;
    Vector3 sagDirection;



    void Start()
    {
        line = GetComponent<LineRenderer>();

        if (!endPointTransform)
        {
            Debug.LogError("No Endpoint Transform assigned to Cable_Procedural component attached to " + gameObject.name);
            return;
        }

        // Sadece sagDirection bir kere hesaplanabilir.
        sagDirection = Physics.gravity.normalized;

        // İlk çizimi Start'ta yapmak yeterli.
        // Draw() metodu Update'te çağrılacağı için burada tekrar çağırmaya gerek yok.
    }

    private void Update()
    {
        // Kabloyu her karede tekrar çiz.
        Draw();
    }



    void Draw()
    {
        if (!endPointTransform || !line)
        {
            return;
        }

        // HER KAREDE YENİDEN HESAPLANMASI GEREKEN DEĞERLER:
        // -------------------------------------------------------------------------------------------------------------------------
        // 1. Get direction Vector and Magnitude (Yeni yön ve uzunluk).
        vectorFromStartToEnd = endPointTransform.position - transform.position;

        // 2. Setting the Start object to look at the end (Kabloyu rüzgara dik tutmak için kullanılıyordu).
        transform.forward = vectorFromStartToEnd.normalized;

        // 3. Get number of points (Yeni uzunluğa göre nokta sayısını ayarla).
        pointsInLineRenderer = Mathf.FloorToInt(pointDensity * vectorFromStartToEnd.magnitude);

        // 4. Set number of points in line renderer (LineRenderer'ı yeni nokta sayısına göre güncelle).
        line.positionCount = pointsInLineRenderer;
        // -------------------------------------------------------------------------------------------------------------------------


        // Ne kadar nokta hesaplanacak
        int i = 0;

        while (i < pointsInLineRenderer)
        {
            // Kablodaki konumuzun yüzdesi
            float pointForCalcs = (float)i / (pointsInLineRenderer - 1);

            // Başlangıçtan bitişe düz çizgideki konum
            Vector3 pointPosition = vectorFromStartToEnd * pointForCalcs;

            // Eğriye göre sarkma miktarı (negatif çünkü sagDirection aşağıya doğruydu, curve değeri pozitif kabul edildi)
            Vector3 sagAtPoint = sagDirection * -curve.Evaluate(pointForCalcs);

            // Kablonun nihai konumu
            Vector3 currentPointsPosition =
            transform.position + // Kablo başlangıç noktası
                pointPosition +     // Düz çizgi üzerindeki konum
                sagAtPoint;         // Sarkma vektörü


            // Noktayı ayarla
            line.SetPosition(i, currentPointsPosition);
            i++;
        }
    }
}
