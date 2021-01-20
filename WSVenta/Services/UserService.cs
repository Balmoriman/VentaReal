using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WSVenta.Models;
using WSVenta.Models.Common;
using WSVenta.Models.Request;
using WSVenta.Models.Response;
using WSVenta.Tools;

namespace WSVenta.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value; 
        }
        public UserResponse Auth(AuthRequestcs model)
        {
            UserResponse userResponse = new UserResponse(); 
            using(var context = new VentaRealContext())
            {
                string clave = Encrypt.GetSHA256(model.Clave);

                var usuario = context.Usuario.Where(d => d.Email == model.Email && d.Clave == clave).FirstOrDefault();

                if (usuario == null) return null; 

                userResponse.Email = usuario.Email;
                userResponse.Token = GetToken(usuario);
            }
            return userResponse; 
        }

        private string GetToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var llave = Encoding.ASCII.GetBytes(_appSettings.Secreto);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier,usuario.Id.ToString()),
                        new Claim(ClaimTypes.NameIdentifier,usuario.Email)
                    }),
                Expires=DateTime.UtcNow.AddDays(60),
                SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(llave),SecurityAlgorithms.HmacSha256Signature)
            };

            //asignar a varaiable 
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); 
        }
    }
}
