using System.Collections.Generic;
using System.Linq;

using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Game.Interaction
{
    public sealed class TimelinePlayer
    {
        private readonly PlayableDirector director;
        private readonly Dictionary<GroupTrack, List<TrackAsset>> groups = new Dictionary<GroupTrack, List<TrackAsset>>();
        private readonly Dictionary<string, GroupTrack> groupNamesGroup = new Dictionary<string, GroupTrack>();

        public TimelinePlayer(PlayableDirector director)
        {
            this.director = director;
        }

        public void PlayGroup(string groupName)
        {
            if (MuteAllButGroupTrack(groupName, out GroupTrack? groupPlay))
            {
                //List<TrackAsset> tracksInGroup = groups[groupPlay];

                /*
                foreach (TrackAsset track in tracksInGroup)
                {
                    var binding = director.GetGenericBinding(track);
                    if (binding == null)
                        continue;
                }*/

                PlayDirector();
            }

            // lokale funktion
            bool MuteAllButGroupTrack(string groupName, out GroupTrack? groupPlay)
            {
                if (groups == null || groups.Count == 0)
                {
                    Init();
                }

                groupPlay = null;
                //print($"GroupTracks Count: {groups.Count}");
                foreach (GroupTrack group in groups.Keys)
                {
                    // still setzen
                    group.muted = !groupName.Equals(group.name);
                    if (!group.muted)
                    {
                        // gruppe gefunden!
                        groupPlay = group;
                        //print(group.name);
                    }
                }
                return groupPlay != null;
            }
        }

        private void PlayDirector()
        {
            // Playable für Track neu abspielen
            director.Stop();
            director.RebuildGraph();
            director.time = 0;
            director.Play();
        }

        private void Init()
        {
            groups.Clear();
            groupNamesGroup.Clear();

            // timeline in playable von director
            TimelineAsset timelineInDirector = (TimelineAsset)director.playableAsset;
            List<TrackAsset> allTracks = timelineInDirector.GetOutputTracks().ToList();

            foreach (TrackAsset track in allTracks)
            {
                GroupTrack group = track.GetGroup();
                if (!groups.TryGetValue(group, out List<TrackAsset> tracks))
                {
                    tracks = new List<TrackAsset>();
                    groups[group] = tracks;
                    groupNamesGroup[group.name] = group;
                }
                tracks.Add(track);
            }
        }
    }
}
