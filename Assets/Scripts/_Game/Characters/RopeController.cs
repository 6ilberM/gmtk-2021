using Obi;
using System.Collections;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public ObiSolver solver;
    public float hookExtendRetractSpeed = 2;
    public Material material;
    public ObiRopeSection section;

    [SerializeField] private PlayerController[] controllers;
    private ObiCollider[] obiRopes;

    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;

    private ObiRopeCursor cursor;

    private void Awake()
    {
        rope = gameObject.AddComponent<ObiRope>();
        ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = section;
        ropeRenderer.uvScale = new Vector2(1, 5);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
        rope.GetComponent<MeshRenderer>().material = material;

        // Setup a blueprint for the rope:
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.resolution = .1f;
        blueprint.thickness = 0.175f;

        // Tweak rope parameters:
        rope.maxBending = 0f;

        obiRopes = new ObiCollider[controllers.Length];
        for (int i = 0; i < controllers.Length; i++)
        { obiRopes[i] = controllers[i].GetComponent<ObiCollider>(); }

    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.xKey.wasPressedThisFrame) { DetachRope(); }
    }

    private void OnDestroy() { DestroyImmediate(blueprint); }

    private IEnumerator AttachRope()
    {
        yield return 0;
        Vector3 localController1 = rope.transform.InverseTransformPoint(controllers[0].transform.position);
        Vector3 localController2 = rope.transform.InverseTransformPoint(controllers[1].transform.position);

        // Procedurally generate the rope path (a simple straight line):
        blueprint.path.Clear();
        blueprint.path.AddControlPoint(localController1, -localController1.normalized, localController1.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "Hook start");
        blueprint.path.AddControlPoint(localController2, -localController2.normalized, localController2.normalized, Vector3.up, 0.1f, 0.1f, 1, 1, Color.white, "Hook end");
        blueprint.path.FlushEvents();

        // Generate the particle representation of the rope (wait until it has finished):
        yield return blueprint.Generate();

        // Set the blueprint (this adds particles/constraints to the solver and starts simulating them).
        rope.ropeBlueprint = blueprint;
        rope.GetComponent<MeshRenderer>().enabled = true;

        // Pin both ends of the rope (this enables two-way interaction between character and rope):
        var pinConstraints = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();
        var batch = new ObiPinConstraintsBatch();

        batch.AddConstraint(rope.solverIndices[0],
                             controllers[0].GetComponent<ObiColliderBase>(),
                            controllers[0].transform.InverseTransformPoint(controllers[0].transform.position),
                            Quaternion.identity,
                            0,
                            0,
                            float.PositiveInfinity);

        batch.AddConstraint(rope.solverIndices[blueprint.activeParticleCount - 1],
                            controllers[1].GetComponent<ObiColliderBase>(),
                            controllers[0].transform.InverseTransformPoint(controllers[0].transform.position),
                            Quaternion.identity,
                            0,
                            0,
                            float.PositiveInfinity);
        batch.activeConstraintCount = 2;
        pinConstraints.AddBatch(batch);

        rope.SetConstraintsDirty(Oni.ConstraintType.Pin);
    }

    private void DetachRope()
    {
        // Set the rope blueprint to null (automatically removes the previous blueprint from the solver, if any).
        rope.ropeBlueprint = null;
        rope.GetComponent<MeshRenderer>().enabled = false;
    }
    public void RopeConnect() { StartCoroutine(AttachRope()); }

    public void DisconnectRope() { DetachRope(); }
}
