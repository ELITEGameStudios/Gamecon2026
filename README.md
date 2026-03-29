**Wind System**

Momentum based currency system designed around rewarding faster players with more movement options without restricting slower players. This system replaces cooldowns.

Wind is generated through horizontal movement. Going up or down does not generate wind.

Wind is represented as a percentage from 0 to 100;

Simply walking will generate wind over time.

Movement options like dashing and wall running will allow you to build more wind.

Blinking, a teleport to the knife's location, can be performed at 100 wind, draining it entirely.
Superdashing, which enables you to dash in the direction you're looking, can be done by holding the dash button down, in exchange for a continuous wind drain. Once you run out of wind, or release the button, the dash ends.

Knife recall speed scales with wind, but drains all remaining wind on pickup. Low wind will make the knife come back slightly slower, high wind will make it come back dramatically faster.

Landing a hit with the knife restores full wind.

**Other Changes**

Blink now re-orients your velocity in the direction you're facing.
Parrying now restores your dash.

**Goals**
1: Reward players who consistently move fast with the resources that they need to accelerate even further

Why this is a goal:
Without the ability to reward players with increasing velocities, the movement loop doesn't change in any positive way over the course of a chain of movement decisions. For example, a player who parries the knife to start a chain at 
100 units/sec will stay at that 100 units per sec, not chasing any future goal, just trying to avoid the punishment of losing their speed.

Achieved by tying wind to move speed, with higher speeds granting more wind.

2: Limit punishment of underperformers who mess up by providing a path to recovery

Why this is a goal:
Players require a safety net to engage with mechanics that are more risky than playing passively. For example, Sekiro's parry mechanic has a safety net of blocking even if the timing is wrong, allowing for players to feel more comfortable
attempting the risky option in the first place. By making the safety net earned (in Sekiro's case, reacting to an attack in the first place), you can still retain most of the skill expression from the mechanic without having to resort to difficulty inflation. If you did use inflation, then you risk the game becoming exhausting to play, as it demands player execution and consistency to both scale upwards constantly, but human tendency is to be inconsistent without practice. As  well, you create a roadmap for the mechanic (i.e. a player who parries 20% of the time in Sekiro is chasing the potential 100%)
Achieved by granting wind even on low speed moves like walking and dashing. Blinking is now much more reliable at saving yourself from a fall thanks to the velocity re-orientation converting it directly into upwards speed if needed.

3: Make dash have more impact on movement decisions

Why this is a goal:
The game has a very limited amount of movement options compared to other games like Ghostrunners or Titanfall. Dash, blinking and parrying are the only ways to consistently accelerate, with wall running being dependent on geometry. If one of these mechanics were too weak, the game could quickly become repetitive to play as a dominant strategy would just be repeated for every situation, which I'll argue is already happening in the base game. See Philosophy.

Achieved through superdashing, which allows dash to function as the precision movement option, where you can carefully manage exactly where you want to go, and how much wind to spend.

4: Make knife recalling downtime decreased for players who consistently engage with movement options that create distance from it, without punishing slower players

Why this is a goal:
Parrying is fundamentally a paradox: in order to parry, you need to get your knife close and recall towards you, but the end result of a parry is knocking your knife away. This creates a problem with incentives that makes some players not engage with parrying unless they're forced to. By tying knife downtime to speed, you can alleviate this issue by still rewarding movement overall with more movement without compromising on the combat aspect of your knife acting as a resource.

Achieved by making knife recalling scales with speed, while leaving the speed at 0 wind relatively similar to the cooldown version.

5: Make it more feasible to maintain high velocity for longer periods of time, without the use of a slide style mechanic that lets you convert air speed to ground speed

Why this is a goal:
The game as it currently does enables a flow based movement system, but uses flow based mechanics. Games like Titanfall 2 are a movement sandbox; they don't require specific actions, just specific results. Options can be exchanged and replaced with sufficient skill. Windweaver is arguably closer to this style; blink only asks you to not hold the knife, and dash only has a 1 time per air entry limiter. Compared to a game like Ghostrunners where movement mechanics such as grappling can only be done on specific grapple points, the game is fairly freeform in nature, but the cooldowns restrict it again.

Achieved by making options that spend wind grant you more velocity or tools to maintain that velocity further. Superdash and blink let you reorient velocity. Dash and parry allow you to accelerate consistently. Recall scaling with wind enables good play to be rewarded with consistent access to the knife. 

Since these tools maintain or increase velocity, you maintain or increase wind build up speed, which enables more usage of these tools, creating a positive feedback loop.

Parry refreshing dash consistently gives you something to do in the air to accelerate (another parry) or re-orient (a superdash, blink)

**Philosophy**

The problem with a cooldown based approach is that it doesn't balance the mechanics out.
While blink is off cooldown, it is objectively the best movement option.
As a result, either the player hoards it, looking for situations that justify the cooldown cost, or use it as soon as it is available, not engaging with other movement mechanics if it is available.
Dash as a result feels like filler, as blink does everything it does better, whether that's range, speed, flexibility, or precision.
Wall running feels punished by this interaction, as you need the blink to chain between walls, but the cooldown limits the length of chaining you can do regardless of how good your execution is. Dash was simply too weak to fill in the gap.
As a result, hoarders can't wall run between walls with even minor distance, and spammers can only chain one or two wall runs without the level design creating extremely large walls to compensate for the cooldown.

Oftentimes cooldowns would be turned off to offset these potential frustrations.

However, this led to the gap between blink and dash being even larger, since blink is still better, but now it's also more available, as dash is based on a limiter (1 time per air visit) instead of a cooldown, rendering it virtually useless.

By making availability tied to performance, fast players can have more ability usage as a reward for their speed.
By maintaining the baseline, slow players are not penalized as hard, while still maintaining incentives to try to get faster.

This also creates a moment to moment positive feedback loop that enables players who go fast by giving them the tools to go even faster.

This loop centers around short term spends on dashes and recalls, to build up to more powerful superdashes and blinks, each strengthening the other.

Superdashing enables the player to spend wind in exchange for re-orienting their momentum. Good for precise movement and minor corrections.
Blinking re-orienting momentum lets blink still save you from falling without the ability to spam it. It also enables it to function more as a combo tool, which makes sense given its high cost.
Knife recalling scaling with speed enables you to consistently chain together parries in order to maintain speed, while making missed recalls still punishing.
Knife hits restoring full wind mirrors cooldown system refreshing recall on kill, enabling players who land shots to increase knife uptime.
Parrying enables rapid acceleration, quickly reaching max wind, or potentially setting up a future blink.


