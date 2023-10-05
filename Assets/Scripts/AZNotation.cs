using System;
using System.Numerics;
using System.Text;

public struct AZNotation
{
    public BigInteger amount;
    private const int ALPHABET_LENGTH = 26;
    private const int MAX_NUMBER_LENGTH = 5; // 정수부 + 소수점 + 실수부 길이
    private const int SPACING = 3;           // 단위 간격 (정수부의 최대 길이)
    private const int MAX_DIGIT_COUNT = 702; // 26 * 27. ZZ로 표현할 수 있는 최대 자리 수

    public readonly string Text
    {
        get
        {
            int digitCount = GetDigitCount();
            BigInteger amount = this.amount;
            StringBuilder result = new();
            int intDigit = digitCount % SPACING;
            if (intDigit == 0)
                intDigit = SPACING;

            int decAmount = 0;
            int chDigitCount = -1;

            int intCount = digitCount - intDigit;
            int divider = (int)Math.Pow(10, SPACING);
            while (intCount > 0)
            {
                if (intCount <= SPACING)
                    decAmount = (int)amount % divider;

                amount /= divider;
                intCount -= SPACING;
                chDigitCount++;
            }

            // 정수부, 실수부
            result.Append($"{amount}");
            if (decAmount != 0)
                result.Append($".{decAmount}");

            // 길이 조절
            while (result.Length > MAX_NUMBER_LENGTH)
                result.Remove(result.Length - 1, 1);

            // 자리수에 맞는 알파벳 추가
            result.Append(GetAZString(chDigitCount));
            return result.ToString();
        }
    }

    // A ~ ZZ
    private readonly string GetAZString(int chIndex)
    {
        if (chIndex < 0 || chIndex >= MAX_DIGIT_COUNT)
            return string.Empty;

        StringBuilder result = new ();
        if (chIndex >= ALPHABET_LENGTH)
        {
            result.Append(GetAZ(chIndex / ALPHABET_LENGTH - 1));
            chIndex %= ALPHABET_LENGTH;
        }
        result.Append(GetAZ(chIndex));
        return result.ToString();
    }

    private readonly char GetAZ(int index)
    {
        return (char)('A' + index);
    }

    private readonly int GetDigitCount()
    {
        int digit = 1;
        BigInteger copyAmount = amount;
        if (copyAmount < 0)
            copyAmount *= -1;
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
}