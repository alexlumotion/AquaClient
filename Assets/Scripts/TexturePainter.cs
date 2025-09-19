using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TexturePainter : MonoBehaviour
{
    [Header("Ціль для малювання")]
    public Renderer targetRenderer;               // Обʼєкт, на якому малюємо
    public Texture2D originalTexture;             // Оригінал (readonly)
    private Texture2D textureToPaint;             // Клон, на якому малюємо

    [Header("Налаштування пензля")]
    public Color brushColor = Color.red;
    public Color[] brushColors;
    public int brushSize = 8;

    private Camera mainCamera;

    // --- NEW: стан для інтерполяції ---
    private bool hasLastUV = false;
    private Vector2 lastUV;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        InitChecks();
        CloneTexture();
        AssignTextureToMaterial();
        EnsureMeshCollider();
        //DrawTestArea(); // можна увімкнути для тесту
    }

    void Update()
    {
        // Початок малювання — фіксуємо першу точку
        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetUVUnderCursor(out var uv))
            {
                PaintAtUV(uv);
                lastUV = uv;
                hasLastUV = true;
            }
        }

        // Продовження малювання — інтерполюємо між lastUV і поточним UV
        if (Input.GetMouseButton(0))
        {
            if (TryGetUVUnderCursor(out var uv))
            {
                if (hasLastUV)
                {
                    PaintLineUV(lastUV, uv); // суцільний мазок
                }
                else
                {
                    PaintAtUV(uv);
                    hasLastUV = true;
                }
                lastUV = uv;
            }
        }

        // Кінець малювання — скидаємо стан
        if (Input.GetMouseButtonUp(0))
        {
            hasLastUV = false;
        }
    }

    // 🧩 Перевірка вхідних параметрів
    void InitChecks()
    {
        if (mainCamera == null)
            Debug.LogError("🚫 TexturePainter: Камера не знайдена.");

        if (originalTexture == null)
            Debug.LogError("🚫 TexturePainter: Не вказана originalTexture.");

        if (targetRenderer == null)
            Debug.LogError("🚫 TexturePainter: Не вказано Renderer.");
    }

    // 🧬 Клонування текстури
    void CloneTexture()
    {
        textureToPaint = new Texture2D(
            originalTexture.width,
            originalTexture.height,
            TextureFormat.RGBA32,
            false
        );

        textureToPaint.SetPixels(originalTexture.GetPixels());
        textureToPaint.Apply();

        Debug.Log($"🧬 Створено клон текстури '{originalTexture.name}' → '{textureToPaint.name}'");
    }

    // 🎨 Привʼязка клону до матеріалу
    void AssignTextureToMaterial()
    {
        targetRenderer.material.mainTexture = textureToPaint;
        Debug.Log("✅ Привʼязано textureToPaint до material.mainTexture");
    }

    // 🧩 MeshCollider setup
    void EnsureMeshCollider()
    {
        MeshCollider meshCol = targetRenderer.GetComponent<MeshCollider>();
        MeshFilter meshFilt = targetRenderer.GetComponent<MeshFilter>();

        if (meshCol != null && meshFilt != null && meshCol.sharedMesh == null)
        {
            meshCol.sharedMesh = meshFilt.sharedMesh;
            Debug.Log("🧩 MeshCollider.mesh встановлено з MeshFilter.sharedMesh");
        }
    }

    // --- NEW: отримаємо UV під курсором без малювання ---
    bool TryGetUVUnderCursor(out Vector2 uv)
    {
        uv = default;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend == targetRenderer)
            {
                uv = hit.textureCoord;
                return true;
            }
        }
        return false;
    }

    // --- NEW: промальовуємо лінію між двома UV з кроком у пікселях ---
    void PaintLineUV(Vector2 uvA, Vector2 uvB)
    {
        if (textureToPaint == null) return;

        // Переведемо у пікселі, щоб крок був відносно розміру пензля
        Vector2 pA = new Vector2(uvA.x * textureToPaint.width,  uvA.y * textureToPaint.height);
        Vector2 pB = new Vector2(uvB.x * textureToPaint.width,  uvB.y * textureToPaint.height);

        float distPx = Vector2.Distance(pA, pB);
        float stepPx = Mathf.Max(1f, brushSize * 0.4f); // ~0.4 радіуса пензля
        int steps = Mathf.CeilToInt(distPx / stepPx);

        // Якщо точки майже співпали — просто намалюємо в кінцевій
        if (steps <= 1)
        {
            PaintAtUV(uvB);
            return;
        }

        // Пройтись рівномірно від A до B
        float inv = 1f / steps;
        for (int s = 1; s <= steps; s++)
        {
            float t = s * inv;
            Vector2 uv = Vector2.Lerp(uvA, uvB, t);
            PaintAtUV(uv);
        }
    }

    // 🎯 Малювання по UV (твоя реалізація — без змін логіки)
    void TryPaintUnderCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend == targetRenderer)
            {
                PaintAtUV(hit.textureCoord);
            }
        }
    }

    // 🖌 Малювання по UV
    void PaintAtUV(Vector2 uv)
    {
        int x = Mathf.RoundToInt(uv.x * textureToPaint.width);
        int y = Mathf.RoundToInt(uv.y * textureToPaint.height);

        float sqrRadius = brushSize * brushSize;

        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (i * i + j * j <= sqrRadius)
                {
                    int px = x + i;
                    int py = y + j;

                    if (px >= 0 && py >= 0 && px < textureToPaint.width && py < textureToPaint.height)
                    {
                        textureToPaint.SetPixel(px, py, brushColor);
                    }
                }
            }
        }

        textureToPaint.Apply();
    }

    public Texture2D GetPaintedTextureCopy()
    {
        Texture2D copy = new Texture2D(textureToPaint.width, textureToPaint.height, textureToPaint.format, false);
        copy.SetPixels(textureToPaint.GetPixels());
        copy.Apply();
        return copy;
    }

    void DrawTestArea()
    {
        for (int x = 0; x < 200; x++)
        {
            for (int y = 0; y < 200; y++)
            {
                textureToPaint.SetPixel(x, y, Color.magenta);
            }
        }
        textureToPaint.Apply();
    }

    public void SetBrushColor(int id)
    {
        brushColor = brushColors[id];
    }

    public void SetFish(Renderer tRender, Texture2D tTexture)
    {
        targetRenderer = tRender;
        originalTexture = tTexture;
        CloneTexture();
        AssignTextureToMaterial();
        EnsureMeshCollider();
    }

    public void Clear()
    {
        CloneTexture();
        AssignTextureToMaterial();
        EnsureMeshCollider();
    }
}