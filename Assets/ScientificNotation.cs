using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Not really proper scientific notation, but it should hopefully do the trick
//to allow *some* kind of math with very large or very small numbers.
//Like the mass of a planetoid.
public struct ScientificNotation
{
    public float number;
    public int exponent;

    public ScientificNotation(float num, int power)
    {
        number = num;
        exponent = power;
    }

    public static explicit operator float(ScientificNotation victim)
    {
        return (victim.number * Mathf.Pow(10, victim.exponent));
    }

    public static ScientificNotation operator *(ScientificNotation lhs, ScientificNotation rhs)
    {
        return new ScientificNotation(lhs.number * rhs.number, lhs.exponent + rhs.exponent);
    }

    public static ScientificNotation operator /(ScientificNotation lhs, ScientificNotation rhs)
    {
        return new ScientificNotation(lhs.number / rhs.number, lhs.exponent - rhs.exponent);
    }

    public static float operator *(float lhs, ScientificNotation rhs)
    {
        return lhs * (float)rhs;
    }
    public static float operator *(ScientificNotation lhs, float rhs)
    {
        return (float)lhs * rhs;
    }

    //Things try to become smaller instead of larger.
    public static ScientificNotation operator +(ScientificNotation lhs, ScientificNotation rhs)
    {
        if(lhs.exponent == rhs.exponent)
        {
            return new ScientificNotation(lhs.number + rhs.number, lhs.exponent);
        } 
        else if(lhs.exponent > rhs.exponent)
        {
            return new ScientificNotation(lhs.number + (float)new ScientificNotation(rhs.number, rhs.exponent - lhs.exponent), lhs.exponent);
        } else
        {
            return new ScientificNotation(rhs.number + (float)new ScientificNotation(lhs.number, lhs.exponent - rhs.exponent), rhs.exponent);
        }
    }
}
