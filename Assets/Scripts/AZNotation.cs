using System;
using System.Numerics;
using System.Text;

public struct AZNotation
{
    public BigInteger amount;
    public readonly string Text
    {
        get
        {
            int maxNumberLength = 5;    // 정수부 + 소수점 + 실수부 길이
            int spacing = 3;            // 단위 간격 (정수부의 최대 길이)

            int total = GetDigit();
            BigInteger amount = this.amount;
            StringBuilder result = new();
            int intDigit = total % spacing;
            if (intDigit == 0)
                intDigit = spacing;

            int decNum = 0;
            int chIndex = -1;

            int intCount = total - intDigit;
            int divider = (int)Math.Pow(10, spacing);
            while (intCount > 0)
            {
                if (intCount <= spacing)
                    decNum = (int)amount % divider;

                amount /= divider;
                intCount -= spacing;
                chIndex++;
            }

            // 정수부, 실수부
            result.Append($"{amount}");
            if (decNum != 0)
                result.Append($".{decNum}");

            // 길이 조절
            while (result.Length > maxNumberLength)
                result.Remove(result.Length - 1, 1);

            // 자리수에 맞는 알파벳 추가
            result.Append(GetAZString(chIndex));
            return result.ToString();
        }
    }

    // A ~ ZZ
    private readonly string GetAZString(int chIndex)
    {
        if (chIndex < 0 || chIndex >= 702) // 최대 자리수 26 * 27
            return string.Empty;

        StringBuilder result = new ();
        int alphabetLength = 26;

        if (chIndex >= alphabetLength)
        {
            result.Append(GetAZ(chIndex / alphabetLength - 1));
            chIndex %= alphabetLength;
        }
        result.Append(GetAZ(chIndex));
        return result.ToString();
    }

    private readonly char GetAZ(int index)
    {
        return (char)('A' + index);
    }

    private readonly int GetDigit()
    {
        int digit = 1;
        BigInteger copyAmount = amount;
        while (copyAmount > 9)
        {
            copyAmount /= 10;
            digit++;
        }
        return digit;
    }

    public static AZNotation operator +(AZNotation a, float b)
    {
        a.amount += (BigInteger)b;
        return a;
    }
    public static AZNotation operator +(AZNotation a, double b)
    {
        a.amount += (BigInteger)b;
        return a;
    }
    public static AZNotation operator +(AZNotation a, int b)
    {
        a.amount += (BigInteger)b;
        return a;
    }
    public static AZNotation operator +(AZNotation a, long b)
    {
        a.amount += (BigInteger)b;
        return a;
    }
    public static AZNotation operator +(AZNotation a, decimal b)
    {
        a.amount += (BigInteger)b;
        return a;
    }
    public static AZNotation operator -(AZNotation a, float b)
    {
        a.amount -= (BigInteger)b;
        return a;
    }
    public static AZNotation operator -(AZNotation a, double b)
    {
        a.amount -= (BigInteger)b;
        return a;
    }
    public static AZNotation operator -(AZNotation a, int b)
    {
        a.amount -= (BigInteger)b;
        return a;
    }
    public static AZNotation operator -(AZNotation a, long b)
    {
        a.amount -= (BigInteger)b;
        return a;
    }
    public static AZNotation operator -(AZNotation a, decimal b)
    {
        a.amount -= (BigInteger)b;
        return a;
    }
}