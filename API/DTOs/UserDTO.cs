using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UserDTO
{
public required string Username{get;set;}
public required string Token{get;set;}
public required string KnownAs{get;set;}

}
