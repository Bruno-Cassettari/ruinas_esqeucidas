using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Leitura de teclado/controle. Funciona tanto com o Input System novo
// (padrão do Unity 6) quanto com o Input Manager antigo.
// Controles: A/D ou setas para andar, Espaço/W/seta para cima para pular, Esc para pausar.
public static class Entrada
{
    public static float Horizontal()
    {
#if ENABLE_INPUT_SYSTEM
        float h = 0f;
        var k = Keyboard.current;
        if (k != null)
        {
            if (k.aKey.isPressed || k.leftArrowKey.isPressed) h -= 1f;
            if (k.dKey.isPressed || k.rightArrowKey.isPressed) h += 1f;
        }
        var g = Gamepad.current;
        if (g != null)
        {
            float x = g.leftStick.x.ReadValue();
            if (Mathf.Abs(x) > 0.3f) h = Mathf.Sign(x);
            if (g.dpad.left.isPressed) h = -1f;
            if (g.dpad.right.isPressed) h = 1f;
        }
        return h;
#else
        return Input.GetAxisRaw("Horizontal");
#endif
    }

    public static bool PuloPressionado()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        var g = Gamepad.current;
        return (k != null && (k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame))
            || (g != null && g.buttonSouth.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
#endif
    }

    public static bool PuloSegurado()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        var g = Gamepad.current;
        return (k != null && (k.spaceKey.isPressed || k.wKey.isPressed || k.upArrowKey.isPressed))
            || (g != null && g.buttonSouth.isPressed);
#else
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
#endif
    }

    public static bool Pausa()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        var g = Gamepad.current;
        return (k != null && k.escapeKey.wasPressedThisFrame) || (g != null && g.startButton.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}
