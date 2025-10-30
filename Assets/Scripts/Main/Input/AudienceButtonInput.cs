using System;

[Serializable]
public struct AudienceButtonInput
{
    public int totalPresses;
    public float deltaPresses;
    public float velocity;

    public AudienceButtonInput(int set_total, float delta_presses, float set_velocity)
    {
        totalPresses = set_total;
        deltaPresses = delta_presses;
        velocity = set_velocity;
    }

    public static AudienceButtonInput None => new AudienceButtonInput(0, 0f, 0f);

    public static AudienceButtonInput GetAxis(AudienceButtonInput negativeButton, AudienceButtonInput positiveButton)
        => new AudienceButtonInput
        (
            positiveButton.totalPresses - negativeButton.totalPresses,
            positiveButton.deltaPresses - negativeButton.deltaPresses,
            positiveButton.velocity - negativeButton.velocity
        );
}