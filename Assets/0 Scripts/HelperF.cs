using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperF : MonoBehaviour {

    // Math Functions
    public static double DClamp(double x, double min, double max) {
        return (x < min) ? min : (x > max) ? max : x;
    }


    // Interpolation Functions
    public static double DLerp(double a, double b, double x) {
        return a * (1 - x) + b * x;
    }

    public static double DInverseLerp(double a, double b, double x) {
        return (x - a) / (b - a);
    }

    public static float FLerp(float a, float b, float x) {
        return a * (1 - x) + b * x;
    }

    public static float FInverseLerp(float a, float b, float x) {
        return (x - a) / (b - a);
    }

    public static float XLerp(float a, float b, float x) {
        return a * (1 - x) + b * x;
    }

    public static float XInverseLerp(float a, float b, float x) {
        return (x - a) / (b - a);
    }


    // Ease Functions
    // More formulas at: https://easings.net/pt-br
    public static double easeLinear(double x) {
        return x;
    }

    public static double easeInQuad(double x) {
        return x * x;
    }

    public static double easeOutQuad(double x) {
        return 1 - (1 - x) * (1 - x);
    }

    public static double easeInCubic(double x) {
        return x * x * x;
    }

    public static double easeOutCubic(double x) {
        return 1 - System.Math.Pow(1 - x, 3);
    }


    // Ease Distance Integrals
    public static double easeLinearDistance(double x) {
        return x / 2;
    }

    public static double easeInQuadDistance(double x) {
        return x / 3;
    }

    public static double easeOutQuadDistance(double x) {
        return x * 2 / 3;
    }

    public static double easeInCubicDistance(double x) {
        return x / 4;
    }

    public static double easeOutCubicDistance(double x) {
        return x * 3 / 4;
    }


    // Normal Difference
    public static float normalDiff(Vector2 normalA, Vector2 normalB) {
        return Mathf.Abs(Vector2.Angle(normalA, normalB));
    }



}
