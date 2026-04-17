**Wind System**

Momentum based currency system designed around rewarding faster players with more movement options without restricting slower players. This system replaces cooldowns.

**Rationale Behind System Implementation**

The cooldown system is designed around creating a scarcity system on top of the 1 hit projectile, primarily with the goal of stopping players from being able to spam options.  The issue is that the game's mechanics are built around flow, so the system-wide mechanics of cooldowns trying to create scarcity is in conflict with the knife mechanics trying to create flow. The goal of the wind system is to move the system mechanics towards the knife's mechanics philosophy.

**Overview**

Wind is generated through horizontal movement. Going up or down does not generate wind.

Wind is represented as a percentage from 0 to 100.

All movement, such as walking, will generate wind over time.

Movement options like dashing and wall running will allow you to build more wind.

Blinking, a teleport to the knife's location, can be performed at 100 wind, draining it entirely.
Superdashing, which enables you to dash in the direction you're looking, can be done by holding the dash button down, in exchange for a continuous wind drain. Once you run out of wind, or release the button, the dash ends.

Knife recall speed scales with wind, but drains all remaining wind on pickup. Low wind will make the knife come back slightly slower, high wind will make it come back dramatically faster.

Wind will change recall speed even if the recall is already active.

Landing a hit with the knife restores full wind.

**Other Changes**

Blink now re-orients your velocity in the direction you're facing.
Parrying now restores your dash.
New ricochet mechanic at 100% wind that enables the knife to bounce off collision points 
Parrying is now one attempt only, but the parry window has been increased from last 30% of travel to last 40%

Parry impulse has been reduced, but parry no longer overwrites your velocity, allowing parry to continuously accelerate you.

Wall jumping now pushes you further and higher from the wall.


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

The addition of ricochet serves as an incentive to maintain 100 wind during combat. Players who consistently move fast are penalized less for aim mistakes, allowing them to prioritize movement over aim safely. Tying the aimbot element of the bounce to map geometry makes aim subservient to the level design the same way that movement is, linking the two together closer. As opposed to other solutions like homing, ricochet retains player agency and autonomy over their knife by still respecting the initial throw, acting as an extension of it, not an alteration. It also allows direct hits while moving fast to still be more rewarding than assisted ones: enemy hits count as collisions, and therefore allow a double kill if two enemies are close enough. Lastly ricochet plays very nicely with the other knife mechanics, AOE parry and damaging recall: you can bounce the knife around to reposition it, setting up bigger parries and recalls. 

Superdashing enables the player to spend wind in exchange for re-orienting their momentum. Good for precise movement and minor corrections.
Blinking re-orienting momentum lets blink still save you from falling without the ability to spam it. It also enables it to function more as a combo tool, which makes sense given its high cost.
Knife recalling scaling with speed enables you to consistently chain together parries in order to maintain speed, while making missed recalls is still punishing.
Knife hits restoring full wind mirrors cooldown system refreshing recall on kill, enabling players who land shots to increase knife uptime.
Parrying enables rapid acceleration, quickly reaching max wind, or potentially setting up a future blink.
Wall running now functions as a battery tool, enabling better movement conservation then running, refreshing your dash unlike parry, and being wind-free unlike blink and superdash. 

Extended wall running is more so for combat encounters, enabling players to keep their current momentum without having to spend wind or players stay on the wall to maintain speed, whereas better players jump, blink and dash from wall to wall to increase speed, utilizing their dash refresh and improved wind economy to convert that velocity into more directions than wall running allows.

As players improve, they begin to engage with the wall running less, instead opting to immediately jump off the wall so they have more control over their positioning, as well as being able to accelerate from the jump. As a result, wall running has been tuned to focus mainly on making that jump useful, rather than the current cooldown version’s continuously running.

Cooldown Vs Wind

Accessibility

Wind is fundamentally more accessible than a cooldown system. The baseline availability of blink can be scaled up to what cooldowns currently allow, while the structure of wind allows for important safety nets to be usable more often for players who consistently move fast and thus take the associated risks that come with that. The system has only two thresholds: greater than 0, which can be achieved with less than a second of walking, and reach 100%. Every mechanic is either locked behind 100%, or is a gradient that gets more powerful the closer you are to 100%.  Rather than ask the player to track multiple different cooldowns (recall, blink, dash, parry) it collapses everything into one resource. Wind also acts as a live performance bar that actively tells players how well they are performing: more wind uptime means more power. In a cooldown system, a correct play looks nearly identical to a wrong one outside of death. For example, blinking to cover due to the overall slower nature of cooldowns may be a correct decision due to other abilities being on cooldown, but may still lead to the player feeling punished as their blink is now on cooldown too, and the Banshee is designed to teleport whenever the player crosses great distances, making them even more vulnerable.

Other mechanics help here as well. Recall speed scaling with wind means that slower players gain a more relaxed parry window. A parry refreshing dash means that you only need to accelerate with parry, not necessarily go in the right direction, because you can still correct later. If you do parry in the direction you want to go, you are rewarded with a better wind economy, so the skill expression is preserved. Ricochet enables a more relaxed aim system by letting players get away with worse aim, which is a natural consequence of moving faster, while still maintaining skill expression through double kills on direct hits and tying aimbot to map geometry the same way movement mechanics are tied to map geometry.

Spectating 

Wind is more exciting for spectators than a cooldown system. This comes from the chaining nature of wind, enabling players to perform longer, more intricate combo systems than cooldowns allow. Even in a situation where a combination is possible in both wind and cooldown, in cooldown, the player may opt not to do it out of fear that their downtime will be punished, whereas the wind player feels compelled to do it because it gives them the resources to avoid downtime. If the game’s appeal comes from visual spectacle, ricochets and longer combo strings increase the game’s volatility enabling for bigger group kills, faster movement, and bigger contrast between players. No matter how big of a spectacle cooldown creates, there is a fixed amount of downtime no player can get around. If the game’s appeal comes from tactical play, wind creates more causality: previous decisions impact future decisions more. Your choice to use a superdash takes away from your ability to use a blink, or vice versa. Where you decide to play during a wave determines where and what is optimal to build wind: a more closed area may incentivize wall running, hazardous areas with things like spikes could promote precise superdash usage, and wide open spaces could promote blinking between map geometry. Cooldowns make individual ability usage independent of each other, making the game less strategic as you simply use every ability when its use case shows up. Without the tools to build speed as often, certain gaps become uncrossable without blink, certain areas become too easy to traverse to risk using blink, and certain mechanics like the Banshee function as a tax on blink that incentivizes hoarding. The game is designed fundamentally for optional movement, with the first few waves disabling most mechanics, so a player will opt to only use that movement when explicitly asked to avoid the risk of not having it when needed.

Feedback

Homing vs Ricochet

Homing was pitched as an aimbot solution that would cause the knife to be animated towards an enemy’s hurtbox, guaranteeing a hit once homing started. This was pitched due to players standing still during playtests with threatless dummies to line up shots. This would not have been tied to wind.

Homing was rejected for the following reasons:

1: It disrespects the player's initial throw. Players who intended for the knife to go to a specific spot during its flight can find their knife going someplace else. Ricochet respects the initial throw and only adds to it afterwards.

2: It lowers aim autonomy. Ricochet expands viable targets to enable more relaxed aim, but still rewards direct hits with double kills. Homing removes the penalty from poorer aim, but doesn’t provide any incentive for accurate shots over accurate enough shots.

3: It is fundamentally unreadable. Ricochet can be predicted to some degree through map geometry: you can’t bounce off a wall or ceiling that isn’t there. This also enables additional tuning that allows for enemies to be harder to kill by placing them in areas with less geometry. Homing doesn’t have any fundamental tells baked into the mechanic, so any attempt to make it readable requires additional art assets, and comes with the risk of making the game more visually cluttered.

4: It doesn’t address the core issue. Players engage with hard material, such as optional bosses in souls-likes, where they are punished for dying, so a player not engaging in movement either may not be the target audience or they’re not being adequately rewarded for doing so. In cooldowns, there is no logical reason to move against a target that is not threatening: it makes your aim worse for no reason. 

5: There were other issues that explained the problem. A placeholder crosshair square was in use that was unreadable, and model based colliders warped harshly with perspective, leading to some view angles having extremely difficult shots. Homing acts as a band-aid solution that doesn’t address the core problem while also coming with its own set of problems.

6: It is not an upgrade. Even if homing was tied to wind to incentivize movement, it still wouldn’t fix the issue. A player who would’ve hit a shot without homing gains nothing from homing, whereas ricochet would reward them with a multi-kill. If homing was tied to 100 wind the way ricochet is, it would be at best a side-grade, which isn’t fitting if the goal is to reward players who made it to the end of the wind loop with additional power to make up for the decreased accuracy from rapid movement.

Cooldown / Wind Hybrid Ver One

Cooldown / Wind Hybrid was pitched as a solution to an issue where players felt they missed the free escape that cooldowns would provide. This would allow players to use a cooldown if they didn’t have the wind for options like blink.

Hybrid was rejected for the following reasons:

1: Cooldowns and Wind have fundamentally different goals. The goal of cooldowns is to create scarcity, while the goal of Wind is to reward movement with abundance. Both mechanics take away from each other due to differentiating philosophies.

2: Skill determines which currency is in effect. A skill floor player may have inconsistent wind and rely on cooldowns to use abilities. The further they get from the skill floor, the less relevant cooldowns become; if they have wind, cooldowns become redundant. 

3: Restricts player agency. People who actively look to get wind are rewarded less because they could’ve gotten the same ability available just by waiting long enough.

4: Wind can be tuned to solve this problem without reintroducing cooldowns. Higher wind on skill floor actions like walking and tap dashing can increase blink availability.

5: Level design can be tuned to solve this problem without reintroducing cooldowns. A player moving too slowly to build wind should be moving too slowly to risk falling. If players are consistently dying at low speeds, the level design is too punishing for low level players and needs to be tweaked to make falling less lethal or happen less often.

6: UI concerns. Creating both systems require both a cooldown display for each ability as well as a wind display, making the game more visually cluttered.

As a result, it would be better to commit to one system. Wind is better than cooldowns for the goals listed in the design document, but a good cooldown system is better than a hybrid compromise.

Cooldown / WInd Hybrid Ver Two

This version would see all mechanics in the game moved back to cooldown except for superdash. This would also see the removal of ricochet, and replaces recall scaling with wind with recall scaling with distance. This tries to solve the same issues, exasperates existing problems with the 1st solution, and creates new ones.

1: Arbitrary values. There isn’t any real reason for why wind is a percent instead of a flat value. The superdash mechanic functions identically outside of duration at 5%, 50%, or 100%.

2: No real goal in the wind system. Without blink or ricochet being tied to 100 wind, there is no real end goal in the system. One of the major strengths of wind is adding a smaller gameplay loop within the major system, but without a goal, you can’t tell when that loop has ended.

3: Unclear on why superdash. Superdash is the only movement mechanic that doesn’t require max wind to use. If It was tied to blink only, it would still function correctly, even if diluted since there is not much decision to be made until you reach 100. 

4: Replacing distance with wind for recall scaling doesn’t account for speed. If I start my recall from close range, but move fast, the knife won’t catch up to my velocity, extending the duration between my recall and being able to parry in a way that feels punishing for fast movement. If I start my recall from far away, and move slower, I get my knife back faster, which incentivizes longer shooting. If the distance boost is continuous (i.e. as the knife gets closer, the knife slows down, rather than a flat boost), then the velocity issue is exacerbated even further by making it a problem with virtually every throw. Longer range shots incentivize less movement to compensate for the fact that targets are smaller. Combine this with wind in and of itself doing less, and the wind as a mechanic becomes not only pointless, but detrimental to a system that rewards slower movement in every other mechanic present in the game. It would be better to remove wind entirely and focus on the scarcity cooldowns provide.

Conclusion

If the goal of the game is to create a set of mechanics and systems that incentivize consistency over peak performance, whether that’s consistently managing a scarce resource between multiple options, or creating a freeflow movement power fantasy centered around chaining options together as long as possible, then wind is the solution. The current game design document creates a set of systems that have little to no restrictions outside of the position of the knife. Blink can be done at any time as long as the knife isn’t held, dash can be done at any time as long as it's the first dash you’ve done in the air, and wall running can be done against any surface for any duration as long as the angle is perpendicular enough. Enemy design incentivizes consistency through the banshee being an unstoppable force of nature and waves extending combat encounters. Every mechanic in the game to some degree points towards a sandbox style of play that cooldown directly contradicts, and wind directly endorses. Any attempt to circumvent consistency by making it less important, whether its homing or re-introducing cooldowns, re-introduces the problems that Wind was designed to solve based on the thesis presented in the game design document. Subjective issues present in Wind, such as going too fast, or being too difficult for beginners, can be fixed within the confines of wind through avenues like increasing skill floor movement wind generation or adjusting level design, without sacrificing what makes it special. There is no better solution than to make the goal of the game, movement consistency, a currency that can be spent to push for more movement.
