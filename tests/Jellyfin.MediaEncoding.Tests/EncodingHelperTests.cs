using System.Collections.Generic;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Model.Entities;
using Xunit;

namespace Jellyfin.MediaEncoding.Tests
{
    public class EncodingHelperTests
    {
        [Fact]
        public void IsFireTvClient_DetectsCommonUserAgents()
        {
            var state = new EncodingJobInfo(TranscodingJobType.Progressive);
            state.RemoteHttpHeaders = new Dictionary<string, string>
            {
                ["User-Agent"] = "Mozilla/5.0 (Linux; Android 9; Fire TV) AppleWebKit/537.36"
            };

            Assert.True(EncodingHelper.TestableIsFireTvClient(state));

            state.RemoteHttpHeaders["User-Agent"] = "Amazon AFTN Build/XYZ";
            Assert.True(EncodingHelper.TestableIsFireTvClient(state));

            state.RemoteHttpHeaders["User-Agent"] = "Some Other Agent";
            Assert.False(EncodingHelper.TestableIsFireTvClient(state));
        }

        [Fact]
        public void ShouldRemoveDynamicHdrMetadata_FireTv_Dv8WithHdr10_RemovesDovi()
        {
            var state = new EncodingJobInfo(TranscodingJobType.Progressive)
            {
                RemoteHttpHeaders = new Dictionary<string, string>
                {
                    ["User-Agent"] = "Amazon AFTN Build/XYZ"
                },
                VideoStream = new MediaStream
                {
                    // Configure fields so GetVideoColorRange() returns VideoRangeType.DOVIWithHDR10
                    Type = MediaStreamType.Video,
                    DvProfile = 8,
                    RpuPresentFlag = 1,
                    BlPresentFlag = 1,
                    DvBlSignalCompatibilityId = 1,
                    Codec = "hevc"
                }
            };

            var plan = EncodingHelper.TestableShouldRemoveDynamicHdrMetadata(state);
            Assert.Equal("RemoveDovi", plan.ToString());
        }
    }
}
