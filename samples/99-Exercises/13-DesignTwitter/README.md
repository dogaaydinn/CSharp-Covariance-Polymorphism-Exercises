# Design Twitter

## Problem
Design a simplified Twitter with core features.

## Requirements
1. Post a tweet
2. Follow/unfollow users
3. Get timeline (tweets from followed users)
4. Like/unlike tweets
5. Retweet

## API Design
```csharp
interface ITwitter
{
    void PostTweet(int userId, string tweet);
    void Follow(int followerId, int followeeId);
    void Unfollow(int followerId, int followeeId);
    List<Tweet> GetTimeline(int userId, int count = 10);
}
```

## Scale Considerations
- Timeline generation (fan-out on write vs read)
- Caching hot user feeds
- Pagination
- Rate limiting

## Data Structures
- User: Dictionary<int, User>
- Tweets: List<Tweet> (sorted by timestamp)
- Followers: Dictionary<int, HashSet<int>>
