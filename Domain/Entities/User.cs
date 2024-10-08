﻿using Domain.Common;
using Domain.Exceptions;
using Domain.Validators;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public sealed class User : IdentityUser<Guid>
{
    private readonly List<Post> _posts = new();
    private readonly List<Like> _likes = new();
    private readonly List<Comment> _comments = new();
    private readonly List<Follow> _followers = new();
    private readonly List<Follow> _following = new();


    public BasicInfo? BasicInfo { get; private set; }
    public string? ProfileImage { get; private set; }
    public int FollowersCount { get; private set; }
    public int FollowingCount { get; private set; }

    public ICollection<Post> Posts => _posts;

    public ICollection<Like> Likes => _likes;

    public ICollection<Comment> Comments => _comments;

    public ICollection<Follow> Followers => _followers; //  Users who follow this user

    public ICollection<Follow> Following => _following; //  Users who this user follows

    public static User CreateUser(string username, string email)
    {
        var validator = new UserValidator();
        var userValidation = new User
        {
            Id = Guid.NewGuid(),
            UserName = username,
            Email = email,
        };
        var validationResult = validator.Validate(userValidation);
        if (validationResult.IsValid) return userValidation;
        var exception = new UserException("User creation failed");
        validationResult.Errors
            .ToList()
            .ForEach(error => exception.ValidationErrors.Add(error.ErrorMessage));

        throw exception;
    }

    // TODO: Edit methods  increment and decrement the followers and following counts in only two functions
    public void IncrementFollowersCount()
    {
        FollowersCount++;
    }

    public void DecrementFollowersCount()
    {
        FollowersCount--;
    }
    public void IncrementFollowingCount()
    {
        FollowingCount++;
    }

    public void DecrementFollowingCount()
    {
        FollowingCount--;
    }

    public void AddProfileImage(string? imageUrl)
    {
        ProfileImage = imageUrl;
    }

    public void UpdateBasicInfo(BasicInfo basicInfo)
    {
        BasicInfo = basicInfo;
    }
}