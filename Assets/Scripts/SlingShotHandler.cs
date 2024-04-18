using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.VFX;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{

    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;


    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;
    [SerializeField] private Transform elesticTransform;

    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float shotForce = 5f;
    [SerializeField] private float timeBetweenBirdRespawns = 2f;
    [SerializeField] private float elesticDivider = 1.2f;
    [SerializeField] private AnimationCurve elasticCurve;
    [SerializeField] private float maxAnimationTime = 1f;





    [SerializeField] private SlingShotArea slingShotArea;
    [SerializeField] private CameraManager cameraManager;

    [SerializeField] private AngieBird angieBirdPreFab;
    [SerializeField] private float angieBirdPositionOffset = 2f;

    private Vector2 slingShotLinesPosition;

    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickedWithinArea;

    private bool birdOnSlingshot;

    private AngieBird spanwedAngieBird;

    private void Awake()
    {
        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        SpawnAngieBird();
    }

    private void Update()
    {

        if (InputManager.WasLeftMouseButtonPressed && slingShotArea.IsWithinslingShotArea())
        {
            clickedWithinArea = true;


            if(birdOnSlingshot)
            {
                //buraya kod gelicek ses icin...

                cameraManager.SwitchToFollowCam(spanwedAngieBird.transform);
            }
        }

        if (InputManager.IsLeftMouseButtonPressed && clickedWithinArea && birdOnSlingshot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }

        if (InputManager.WasLeftMouseButtonReleased && birdOnSlingshot && clickedWithinArea)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                clickedWithinArea = false;
                birdOnSlingshot = false;

                spanwedAngieBird.LaucnhBird(direction, shotForce);

                GameManager.instance.UseShot();

                AnimeteSlingShot();



                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngieBirdAfterTime());
                }


            }

        }
    }

    #region SlingShoy Methods

    private void DrawSlingShot()
    {


        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.mousePosition);

        slingShotLinesPosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);

        SetLines(slingShotLinesPosition);

        direction = (Vector2)centerPosition.position - slingShotLinesPosition;
        directionNormalized = direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!leftLineRenderer.enabled && !rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;
        }


        leftLineRenderer.SetPosition(0, position);
        leftLineRenderer.SetPosition(1, leftStartPosition.position);

        rightLineRenderer.SetPosition(0, position);
        rightLineRenderer.SetPosition(1, rightStartPosition.position);
    }

    #endregion



    #region Angie Bird Methods


    private void SpawnAngieBird()
    {

        elesticTransform.DOComplete();
        SetLines(idlePosition.position);

        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * angieBirdPositionOffset;

        spanwedAngieBird = Instantiate(angieBirdPreFab, spawnPosition, Quaternion.identity);
        spanwedAngieBird.transform.right = dir;

        birdOnSlingshot = true;

    }

    private void PositionAndRotateAngieBird()
    {
        spanwedAngieBird.transform.position = slingShotLinesPosition + directionNormalized * angieBirdPositionOffset;
        spanwedAngieBird.transform.right = directionNormalized;
    }

    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(timeBetweenBirdRespawns);

        SpawnAngieBird();

        cameraManager.SwitchToIdleCam();
    }

    #endregion


    #region Animate SlingShot

    private void AnimeteSlingShot()
    {

        elesticTransform.position = leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(elesticTransform.position, centerPosition.position);

        float time = dist / elesticDivider;

        elesticTransform.DOMove(centerPosition.position, time).SetEase(elasticCurve);
        StartCoroutine(AnimateSlingShotLines(elesticTransform, time));
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time && elapsedTime < maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;


            SetLines(trans.position);

            yield return null;
        }
    }

    #endregion
}
