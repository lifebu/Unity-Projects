using UnityEngine;

public static class CanvasExtensions
{
    /// <summary>
	/// Uses a canvas-space position to create a Ray from the Canvas into the World
	/// </summary>
	/// <param name='position'> The canvas-space position to create the Ray from. </param>
    public static Ray CanvasCoordToRay(this Canvas canvas, Vector2 position)
    {
        Rect canvRect = canvas.GetComponent<RectTransform>().rect;
        return Camera.main.ViewportPointToRay(new Vector3(position.x / canvRect.width, position.y / canvRect.height));
    }

    /// <summary>
	/// Uses a Canvas-Space Coordinate to cast a Ray from the Canvas into the World and return the RaycastHit
	/// </summary>
	/// <param name='position'> The point to debug. </param>
    /// <param name='hit'> returns the hit information of this Raycast. </param>
    /// <param name='maxDistance'> The max distance the Ray is allowed to be from the start of the ray. </param>
    /// <param name='layerMask'> Ignore colliders that are not of this layer. </param>
    /// <returns> true == we had a hit. </returns>
    public static bool CastRayFromCanvas(this Canvas canvas, Vector2 position, out RaycastHit hit, 
        float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
    {
        Ray ray = canvas.CanvasCoordToRay(position);
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {

            return true;
        }


        return false;
    }

    /// <summary>
	/// Uses a Canvas-Space Coordinate to cast a Ray from the Canvas into the World and return the RaycastHit
	/// </summary>
	/// <param name='position'> The point to debug. </param>
    /// <param name='hitPosition'> returns the position where the hit was detected. </param>
    /// <param name='maxDistance'> The max distance the Ray is allowed to be from the start of the ray. </param>
    /// <param name='layerMask'> Ignore colliders that are not of this layer. </param>
    /// <returns> true == we had a hit. </returns>
    public static bool CastRayFromCanvas(this Canvas canvas, Vector2 position, out Vector3 hitPosition, 
        float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
    {
        Ray ray = canvas.CanvasCoordToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            hitPosition = hit.point;
            return true;
        }

        hitPosition = new Vector3();
        return false;
    }

    /// <summary>
    /// This will convert a position from Canvas-Space to World-Space.
    /// </summary>
    /// <param name="position"> the Position in Canvas-Space. </param>
    /// <returns> A new position in World-Space</returns>
    public static Vector3 CanvasToWorldPoint(this Canvas canvas, Vector2 position)
    {
        Rect canvRect = canvas.GetComponent<RectTransform>().rect;
        // get the Viewportposition of this Canvasposition by dividing the Components with the dimension of the canvas.
        Vector2 viewportPos = new Vector2(position.x / canvRect.width, position.y / canvRect.height);
        // Now you can use the Maincamera to convert this into a Worldposition
        return Camera.main.ViewportToWorldPoint(viewportPos);
    }

    /// <summary>
    /// This will convert a position from World-Space to Canvas-Space.
    /// </summary>
    /// <param name="position"> The position in World-Space. </param>
    /// <returns> A new position in Canvas-Space.</returns>
    public static Vector2 WorldToCanvasPoint(this Canvas canvas, Vector3 position)
    {
        // get the Viewportposition of this Worldposition
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(position);
        Rect canvRect = canvas.GetComponent<RectTransform>().rect;
        // Now multiply the dimensions to this ViewportPos to get a Vector in the Canvas-Space and return that.
        return new Vector3(viewportPos.x * canvRect.width, viewportPos.y * canvRect.height);
    }

    /// <summary>
    /// This will convert a position from Screen-Space to Canvas-Space.
    /// </summary>
    /// <param name="worldPos"> The position in Screen-Space. </param>
    /// <returns> A new position in Canvas-Space.</returns>
    public static Vector2 ScreenToCanvasPoint(this Canvas canvas, Vector2 position)
    {
        // get the Viewportposition of this Worldposition
        Vector3 viewportPos = Camera.main.ScreenToViewportPoint(position);
        Rect canvRect = canvas.GetComponent<RectTransform>().rect;
        // Now multiply the dimensions to this ViewportPos to get a Vector in the Canvas-Space and return that.
        return new Vector3(viewportPos.x * canvRect.width, viewportPos.y * canvRect.height);
    }
}
