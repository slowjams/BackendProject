﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Core.Users.Commands;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Core.Users.Queries;

namespace WebApplication.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class UsersController : ControllerBase
   {
      private readonly IMediator _mediator;

      public UsersController(IMediator mediator)
      {
         _mediator = mediator;
      }

      [HttpGet]
      [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
      public async Task<IActionResult> GetUserAsync(
          [FromQuery] GetUserQuery query,
          CancellationToken cancellationToken)
      {
         UserDto result = await _mediator.Send(query, cancellationToken);
         return Ok(result);
      }

      // TODO: create a route (at /Find) that can retrieve a list of matching users using the `FindUsersQuery`
      [HttpGet("/Find")]
      [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<IActionResult> FindUsersAsync(
          [FromQuery] FindUsersQuery query,
          CancellationToken cancellationToken)
      {    
         IEnumerable<UserDto> result = await _mediator.Send(query, cancellationToken);
         return Ok(result);
      }

      //// TODO: create a route (at /List) that can retrieve a paginated list of users using the `ListUsersQuery`
      [HttpGet("/List")]
      [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
      public async Task<IActionResult> ListUsersAsync(
         [FromQuery] ListUsersQuery query,
         CancellationToken cancellationToken)
      {
         var result = await _mediator.Send(query, cancellationToken);     
         return Ok(result.Data);
      }

      // TODO: create a route that can create a user using the `CreateUserCommand`
      [HttpPost]
      [ProducesResponseType(StatusCodes.Status201Created)]
      //[ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<IActionResult> CreateUserAsync(
          [FromBody] CreateUserCommand query,
          CancellationToken cancellationToken)
      {
         var result = await _mediator.Send(query, cancellationToken);
         //return CreatedAtAction(nameof(CreateUserAsync), new { id = result.UserId }, result);
         return CreatedAtAction(nameof(CreateUserAsync), result);
      }

      //// TODO: create a route that can update an existing user using the `UpdateUserCommand`
      [HttpPut]
      [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
      public async Task<IActionResult> UpdateUserAsync(
          [FromBody] UpdateUserCommand query,
          CancellationToken cancellationToken)
      {
         await _mediator.Send(query, cancellationToken);      
         return NoContent();
      }

      // TODO: create a route that can delete an existing user using the `DeleteUserCommand`
      [HttpDelete]
      public async Task<IActionResult> DeleteUserAsync(
          [FromBody] DeleteUserCommand query,
          CancellationToken cancellationToken)
      {
         await _mediator.Send(query, cancellationToken);
         return NoContent();
      }
   }
}
