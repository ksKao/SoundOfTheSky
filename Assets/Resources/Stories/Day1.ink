EXTERNAL ChangeScene(sceneType)
EXTERNAL FadeBackground(fileName, duration)
EXTERNAL Delay(duration)
EXTERNAL PlayAudio(fileName)
EXTERNAL LoopAudio(fileName)
EXTERNAL SetAudioVolume(fileName, volume)
EXTERNAL FadeAudioVolume(fileName, volume, duration)
EXTERNAL StopAudio(fileName)

~ ChangeScene("Title")

Chapter 1 Day 1

~ ChangeScene("Subtitle")

It's been years since the civil war ended, but Alaska's scars linger along with the rest of America. For us, cities and villages lie half-buried under heavy snow, their chimneys cold, while a new capital stands revitalized, a distant beacon of order. A new railroad snakes through the wild to reconnect the isolated and rebuild the broken. At first glance, these trains might seem a relic of the past, but are in fact our present. However remnants of the older world persist: fragments of high-speed rails, social media echoes, and sports magazines depicting large crowds gathering to see their favorite athletes. The days of bustling malls and divisive politics are gone, replaced by a fragile unity born of necessity. #title=Year: 2050:

We're all under one sky, no matter the noise below. A sound from from the sky might remind us we're not as divided as we think.” -Olga Orlov

~ ChangeScene("Blank")
~ FadeBackground("villagers_color", 5)
~ LoopAudio("dragonwind")
~ SetAudioVolume("dragonwind", 1)
~ Delay(5)
~ ChangeScene("Dialog")

An arctic village clings to life like a dying ember, its snow-choked homes casting no smoke. The sky hangs dull gray, swallowing shadows. Thin figures stand in the drifts, bundled in layers piled high against the artic cold, their breath clouding in the biting air.

A mournful wind cuts through the silence, broken only by occasional distant chatter.

~ SetAudioVolume("dragonwind", 0.2)

[Looking down, wringing her hands. Her voice trembles, eyes scanning the white horizon.] #speaker=Sara #portrait=sara_half

When are they coming? #speaker=Sara #portrait=sara_half

[Shivering, clutching a thin blanket, tugging at two nearby adults. His lips are chapped from the cold.] #speaker=Tim #portrait=tim_half

Are they really coming? #speaker=Tim #portrait=tim_half

[Kneeling, forcing a smile, though his eyes betray doubt.] #speaker=Frank #portrait=frank_half

Don't worry, Tim, I'm sure they'll come. [His hand rests on Tim's head.] #speaker=Frank #portrait=frank_half

[Arms crossed, staring at the ground, voice flat and resigned.] #speaker=Bobby #portrait=bobby_half

Hmph. The capital starts the mess, then takes its sweet time fixing it. Bet they've forgotten us. #speaker=Bobby #portrait=bobby_half

[Hunched slightly, shivering to generate any warmth possible.] #speaker=Mara #portrait=mara_half

We're out of supplies and options. All we can do is stay optimistic and wait. #speaker=Mara #portrait=mara_half

[Snapping, fists clenched.] #speaker=Bobby #portrait=bobby_half

It's this damn war's fault! We didn't ask for it, yet we're the ones suffering. One idiot glares at another, and next thing you know, we're all buried in snow. #speaker=Bobby #portrait=bobby_half

His words echo briefly, then fade into the drifts.

[Huddling closer to Sara, his small voice lost in the wind.] #speaker=Tim #portrait=tim_half

I'm cold… and hungry… #speaker=Tim #portrait=tim_half

[Standing tall, voice booming despite his frail frame.] #speaker=Arthur #portrait=arthur

Everyone! Let's gather and pray for a new life, for the seasons to turn again, for a sign from above to carry us forward! #speaker=Arthur #portrait=arthur

He raises his hands, inviting the villagers closer. Villagers shuffle closer, forming a rough circle. Hands clasp, lips move silently as snow dances around them.

~ SetAudioVolume("dragonwind", 1)

The wind howls louder, drowning murmurs. Then… silence... A deafening void swallows the village.

~ SetAudioVolume("dragonwind", 0.2)

[Looking over the crowd with warm, steady eyes.] #speaker=Arthur #portrait=arthur

My friends, my family, set aside your burdens for a moment. I know we're tired and scared. The days are long, and help feels far. But we are not alone, not now, not ever. The Lord is with us, calling us to stand together. Will you pray with me? #speaker=Arthur #portrait=arthur

Villagers nod, some clasping hands, others bowing heads. Arthur raises his chin, signaling the start of the prayer.

Heavenly Father, in this hour of trial, we turn to You, our rock and refuge. Grant us strength to endure, courage to face our fears, and faith to trust Your plan. Surround us with Your love, unite us in hope, and guide us through this storm. May Your peace fill our hearts as we await Your deliverance. Amen. #speaker=Arthur #portrait=arthur

~ ChangeScene("Blank")
~ FadeBackground("snowy_village_color", 0.5)
~ StopAudio("dragonwind")
~ LoopAudio("cold_strong_wind")
~ Delay(10)
~ FadeAudioVolume("cold_strong_wind", 0, 5)
~ Delay(15)
~ ChangeScene("Dialog")
~ StopAudio("cold_strong_wind")

The prayer fades, buried in silence. A cold gust sweeps through, as if the sky itself presses down on the world.

~ ChangeScene("Blank")
~ FadeBackground("black", 5)

