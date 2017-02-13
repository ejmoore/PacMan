using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// Called by PlaySpaceManager after planes have been generated from the Spatial Mapping Mesh.
/// This class will create a collection of prefab objects that have the 'Placeable' component and
/// will attempt to set their initial location on planes that are close to the user.
/// </summary>
public class SpaceCollectionManager : Singleton<SpaceCollectionManager>
{
    [Tooltip("A collection of Placeable space object prefabs to generate in the world.")]
    public List<GameObject> spaceObjectPrefabs;

    public int numOfSpheres = 80;
    public GameObject myCamera;
    public GameObject ghost;
    public bool ghostActive = true;
    public int numOfSpheresToBePlaced = 5; //Maximum number of spheres that will be placed before finishing
    public float minDistanceBetween = 2; //Minimum distance between spheres
    Vector3[] positions;

    public int placedSpheres = 0;

    public GameObject stats;
    TimeManager timeManager = null;

    void Start()
    {
        positions = new Vector3[numOfSpheres];
        timeManager = stats.GetComponent<TimeManager>();
    }

    public void removeSphere(Vector3 sphere)
    {
        for (int i = 0; i < placedSpheres; i++)
        {
            if (Vector3.Distance(sphere, positions[i]) == 0)
            {
                timeManager.GetPoint(positions[i]);
                positions[i] = new Vector3();
                return;
            }
        }
    }

    /// <summary>
    /// Generates a collection of Placeable objects in the world and sets them on planes that match their affinity.
    /// </summary>
    /// <param name="horizontalSurfaces">Horizontal surface planes (floors, tables).</param>
    /// <param name="verticalSurfaces">Vertical surface planes (walls).</param>
    public void GenerateItemsInWorld(List<GameObject> horizontalSurfaces, List<GameObject> verticalSurfaces)
    {
        List<GameObject> horizontalObjects = new List<GameObject>();
        List<GameObject> verticalObjects = new List<GameObject>();

        foreach (GameObject spacePrefab in spaceObjectPrefabs)
        {
            Placeable placeable = spacePrefab.GetComponent<Placeable>();
            if (placeable.PlacementSurface == PlacementSurfaces.Horizontal)
            {
                horizontalObjects.Add(spacePrefab);
            }
            else
            {
                verticalObjects.Add(spacePrefab);
            }
        }

        if (horizontalObjects.Count > 0)
        {
            CreateSpaceObjects(horizontalObjects, horizontalSurfaces, PlacementSurfaces.Horizontal);
        }

        if (verticalObjects.Count > 0)
        {
            CreateSpaceObjects(verticalObjects, verticalSurfaces, PlacementSurfaces.Vertical);
        }
        if (ghostActive) ghost.SetActive(true); //Make the Ghost follow the player
    }

    /// <summary>
    /// Creates and positions a collection of Placeable space objects on SurfacePlanes in the environment.
    /// </summary>
    /// <param name="spaceObjects">Collection of prefab GameObjects that have the Placeable component.</param>
    /// <param name="surfaces">Collection of SurfacePlane objects in the world.</param>
    /// <param name="surfaceType">Type of objects and planes that we are trying to match-up.</param>
    private void CreateSpaceObjects(List<GameObject> spaceObjects, List<GameObject> surfaces, PlacementSurfaces surfaceType)
    {
        List<int> UsedPlanes = new List<int>();
        placedSpheres = 0;

        // Sort the planes by distance to user.
        surfaces.Sort((lhs, rhs) =>
        {
            Vector3 headPosition = Camera.main.transform.position;
            Collider rightCollider = rhs.GetComponent<Collider>();
            Collider leftCollider = lhs.GetComponent<Collider>();

            // This plane is big enough, now we will evaluate how far the plane is from the user's head.  
            // Since planes can be quite large, we should find the closest point on the plane's bounds to the 
            // user's head, rather than just taking the plane's center position.
            Vector3 rightSpot = rightCollider.ClosestPointOnBounds(headPosition);
            Vector3 leftSpot = leftCollider.ClosestPointOnBounds(headPosition);

            return Vector3.Distance(leftSpot, headPosition).CompareTo(Vector3.Distance(rightSpot, headPosition));
        });


        //debug:
        float minDistance = 10000;

        foreach (GameObject item in spaceObjects)
        {
            int index = -1;
            Collider collider = item.GetComponent<Collider>();

            if (surfaceType == PlacementSurfaces.Vertical)
            {
                index = FindNearestPlane(surfaces, collider.bounds.size, UsedPlanes, true);
            }
            else
            {
                index = FindNearestPlane(surfaces, collider.bounds.size, UsedPlanes, false);
            }

            // If we can't find a good plane we will put the object floating in space.
            Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 2.0f + Camera.main.transform.right * (Random.value - 1.0f) * 2.0f;
            Vector3 badPosition = position;
            Quaternion rotation = Quaternion.identity;

            // If we do find a good plane we can do something smarter.
            if (index >= 0)
            {
                UsedPlanes.Add(index);
                GameObject surface = surfaces[index];
                SurfacePlane plane = surface.GetComponent<SurfacePlane>();
                position = surface.transform.position + (plane.PlaneThickness * plane.SurfaceNormal);
                //position = AdjustPositionWithSpatialMap(position, plane.SurfaceNormal);
                rotation = Camera.main.transform.localRotation;

                if (surfaceType == PlacementSurfaces.Vertical)
                {
                    // Vertical objects should face out from the wall.
                    rotation = Quaternion.LookRotation(surface.transform.forward, Vector3.up);
                }
                else
                {
                    // Horizontal objects should face the user.
                    rotation = Quaternion.LookRotation(Camera.main.transform.position);
                    rotation.x = 0f;
                    rotation.z = 0f;
                }


            }
            else
            {
                continue;
            }


            GameObject spaceObject = Instantiate(item, position, rotation) as GameObject; //Create a new GameObject on the selected plane
            spaceObject.transform.position -= spaceObject.transform.forward.normalized * 1.0f; //Move the GameObject 1 meter away from the plane
            Vector3 temp = new Vector3(0, spaceObject.transform.position.y, 0);
            spaceObject.transform.position -= temp; //Set the y value of the GameObject to be 0
            for (int i = 0; i < numOfSpheres; i++) //Loop through all current positions of GameObjects
            {

                //debug:
                minDistance = 10000000;

                if (Vector3.Distance(spaceObject.transform.position, badPosition) <= .1)
                {
                    spaceObject.SetActive(false);
                    break;
                }
                if (positions[i] != new Vector3() || Vector3.Distance(positions[i], new Vector3()) > .01)
                {

                    //debug:
                    if (Vector3.Distance(spaceObject.transform.position, positions[i]) < minDistance)
                    {
                        minDistance = Vector3.Distance(spaceObject.transform.position, positions[i]);
                    }

                    if (Vector3.Distance(spaceObject.transform.position, positions[i]) < minDistanceBetween) //Set the GameObject to inactive if it's too close to another GameObject
                    {
                        spaceObject.SetActive(false);
                        break;
                    }
                }
                else //Add the GameObject to positions while leaving it active
                {
                    if (Vector3.Distance(spaceObject.transform.position, myCamera.transform.position) < 2)
                    {
                        //debug:
                        //Debug.Log("Too close to the camera");
                        spaceObject.SetActive(false);
                    }
                    else
                    {

                        positions[i] = spaceObject.transform.position;
                        placedSpheres++;
                        //debug:
                        //Debug.Log("Distance placed from player: " + Vector3.Distance(positions[i], myCamera.transform.position));
                    }
                    break;
                }
            }
            spaceObject.transform.parent = gameObject.transform;
            //debug:
            //Debug.Log(minDistance);

            if (placedSpheres == numOfSpheresToBePlaced)
            {
                GameObject.Find("Stats").GetComponent<TimeManager>().StartTimer();
                break;
            }
        }
        //debug:
        //Debug.Log("placed spheres:" + placedSpheres);

    }

    /// <summary>
    /// Attempts to find a the closest plane to the user which is large enough to fit the object.
    /// </summary>
    /// <param name="planes">List of planes to consider for object placement.</param>
    /// <param name="minSize">Minimum size that the plane is required to be.</param>
    /// <param name="startIndex">Index in the planes collection that we want to start at (to help avoid double-placement of objects).</param>
    /// <param name="isVertical">True, if we are currently evaluating vertical surfaces.</param>
    /// <returns></returns>
    private int FindNearestPlane(List<GameObject> planes, Vector3 minSize, List<int> usedPlanes, bool isVertical)
    {
        int planeIndex = -1;

        for (int i = 0; i < planes.Count; i++)
        {
            if (usedPlanes.Contains(i))
            {
                continue;
            }

            Collider collider = planes[i].GetComponent<Collider>();
            if (isVertical && (collider.bounds.size.x < minSize.x || collider.bounds.size.y < minSize.y))
            {
                // This plane is too small to fit our vertical object.
                continue;
            }
            else if (!isVertical && (collider.bounds.size.x < minSize.x || collider.bounds.size.y < minSize.y))
            {
                // This plane is too small to fit our horizontal object.
                continue;
            }

            return i;
        }

        return planeIndex;
    }

    /// <summary>
    /// Adjusts the initial position of the object if it is being occluded by the spatial map.
    /// </summary>
    /// <param name="position">Position of object to adjust.</param>
    /// <param name="surfaceNormal">Normal of surface that the object is positioned against.</param>
    /// <returns></returns>
    private Vector3 AdjustPositionWithSpatialMap(Vector3 position, Vector3 surfaceNormal)
    {
        Vector3 newPosition = position;
        RaycastHit hitInfo;
        float distance = 0.5f;

        // Check to see if there is a SpatialMapping mesh occluding the object at its current position.
        if (Physics.Raycast(position, surfaceNormal, out hitInfo, distance, SpatialMappingManager.Instance.LayerMask))
        {
            // If the object is occluded, reset its position.
            newPosition = hitInfo.point;
        }

        return newPosition;
    }

    public Vector3 findNearestSphere()
    {
        if (placedSpheres == 0)
        {
            return Vector3.one;
        }

        Vector3 shortestSphere = myCamera.transform.position;

        for (int j = 0; j < numOfSpheres; j++)
        {
            if (positions[j] == new Vector3())
            {
                continue;
            }
            if (shortestSphere.Equals(myCamera.transform.position))
            {
                shortestSphere = positions[j];
            }
            if (Vector3.Distance(myCamera.transform.position, positions[j]) < Vector3.Distance(shortestSphere, myCamera.transform.position))
            {
                shortestSphere = positions[j];
            }
        }
        return shortestSphere;
    }
}
