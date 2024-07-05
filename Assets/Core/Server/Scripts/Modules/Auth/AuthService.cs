using System;
using System.Security.Cryptography;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using UnityEngine;

namespace MMC.Server
{
    [Service]
    public class AuthService : Service
    {
        private RSA rsaKey;
        private string secret = "secret";

        public override void Build()
        {
            base.Build();
            rsaKey = RSA.Create();
            if (PlayerPrefs.HasKey("rsa_key"))
            {
                var data = PlayerPrefs.GetString("rsa_key");
                rsaKey.FromXmlString(data);
            }
            else
            {
                PlayerPrefs.SetString("rsa_key", rsaKey.ToXmlString(true));
            }
        }

        public string EncodeToken(Action<JwtBuilder> useBuilder)
        {
            var builder = JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(rsaKey, rsaKey))
                .WithSecret(secret);
            // .ExpirationTime(DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
            // .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds());
            useBuilder.Invoke(builder);
            var token = builder.Encode();
            return token;
        }
        public string DecodeToken(string token)
        {
            var json = JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(rsaKey))
                .Decode(token);
            return json;
        }
        public bool VerifyToken(string token) => VerifyToken(token, out _);
        public bool VerifyToken(string token, out string json)
        {
            json = "";
            try
            {
                json = JwtBuilder.Create()
                    .WithAlgorithm(new RS256Algorithm(rsaKey))
                    .MustVerifySignature()
                    .Decode(token);
                return true;
            }
            catch (TokenNotYetValidException) { return false; }
            catch (TokenExpiredException) { return false; }
            catch (SignatureVerificationException) { return false; }
            catch (Exception) { return false; }
        }
    }
}