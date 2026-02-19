using System.Text.RegularExpressions;

namespace OrgChart.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um endereço de email válido
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Address { get; private set; }

    private Email(string address)
    {
        Address = address;
    }

    public static Email Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Email não pode ser vazio", nameof(address));

        address = address.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(address))
            throw new ArgumentException($"Email '{address}' é inválido", nameof(address));

        return new Email(address);
    }

    public override string ToString() => Address;

    public override bool Equals(object? obj) => obj is Email email && Equals(email);

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Address.Equals(other.Address, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => Address.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(Email? left, Email? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Email? left, Email? right) => !(left == right);

    public static implicit operator string(Email email) => email.Address;
}
